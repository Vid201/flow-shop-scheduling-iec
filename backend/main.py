from flask import Flask, request, jsonify
from mmas import PermutationFlowShopScheduling, MaxMinAntSystem
from multiprocessing import Process
import numpy as np
import time
import uuid

app = Flask(__name__)

NUMBER_OF_ITERATIONS = 1
MAX_ITERATIONS = 100
RESULTS_FOLDER = "results/"

games = {}


def optimal_solution(game_id, pfss):
    with open(f"{RESULTS_FOLDER}{game_id}.optimal", 'w') as file:
        start_time = time.time()
        sol = pfss.get_optimal_solution()
        end_time = time.time()
        file.write(f"{sol}\n{end_time - start_time} s")


def mmas_solution(game_id):
    with open(f"{RESULTS_FOLDER}{game_id}.mmas", 'w') as file:
        file_content = ""

        start_time = time.time()
        for i in range(MAX_ITERATIONS):
            games[game_id].run(iters=NUMBER_OF_ITERATIONS)
            file_content += f"{i+1}. {games[game_id].get_best_solution()}\n"
        end_time = time.time()
        file.write(f"{file_content}{end_time - start_time} s")


def interactive_mmas_solution(game_id):
    with open(f"{RESULTS_FOLDER}{game_id}.interactive", 'a') as file:
        games[game_id].run(iters=NUMBER_OF_ITERATIONS)
        sol = games[game_id].get_best_solution()
        file.write(f"{games[game_id].iter}. {sol}\n")

    return sol

# send game data at the beginning
# in: game data (number of jobs, number of machines, jobs data)
# out: game_id, suggestion


@app.route("/game", methods=["POST"])
def game():
    seed = np.random.randint(0, 10000)

    number_of_jobs = int(request.form['numberOfJobs'])
    number_of_machines = int(request.form['numberOfMachines'])
    times_str = request.form['times'].strip()
    times = np.asarray(list(map(float, times_str.split(' ')))).reshape(
        (number_of_jobs, number_of_machines))

    np.random.seed(seed)

    pfss = PermutationFlowShopScheduling(
        number_of_machines=number_of_machines, number_of_jobs=number_of_jobs, times=times)
    mmas = MaxMinAntSystem(pfss)
    id = str(uuid.uuid4())
    games[id] = mmas

    optimal_process = Process(target=optimal_solution,
                              daemon=True, args=[id, pfss])
    optimal_process.start()

    mmas_solution(id)
    del games[id]

    np.random.seed(seed)

    mmas2 = MaxMinAntSystem(pfss)
    games[id] = mmas2

    interactive_mmas_solution(id)

    return jsonify({
        "gameId": id,
        "suggestion": ' '.join(map(str, games[id].get_best_solution()[0]))
    })

# send user move
# in: game_id, action (add, remove), job_id, index
# out: suggestion


@app.route("/move/<game_id>", methods=["POST"])
def move(game_id):
    action = request.form['action']
    job_id = int(request.form['jobId'])
    index = int(request.form['index'])

    games[game_id].change_pheromone(
        job_id, index, True if action == "add" else False)
    interactive_mmas_solution(game_id)

    return jsonify({
        "gameId": game_id,
        "suggestion": ' '.join(map(str, games[game_id].get_best_solution()[0]))
    })


if __name__ == '__main__':
    app.run()
