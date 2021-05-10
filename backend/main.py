from flask import Flask, request, jsonify
from mmas import PermutationFlowShopScheduling, MaxMinAntSystem
import numpy as np

app = Flask(__name__)

NUMBER_OF_ITERATIONS = 3

counter = 0
games = {}


# send game data at the beginning
# in: game data (number of jobs, number of machines, jobs data)
# out: game_id, suggestion


@app.route("/game", methods=["POST"])
def game():
    global counter

    number_of_jobs = int(request.form['numberOfJobs'])
    number_of_machines = int(request.form['numberOfMachines'])
    times_str = request.form['times'].strip()
    times = np.asarray(list(map(float, times_str.split(' ')))).reshape((number_of_jobs, number_of_machines)) 

    pfss = PermutationFlowShopScheduling(number_of_machines=number_of_machines, number_of_jobs=number_of_jobs, times=times)
    mmas = MaxMinAntSystem(pfss)

    games[counter] = mmas
    counter += 1

    games[counter - 1].run(iters=NUMBER_OF_ITERATIONS)

    return jsonify({
        "gameId" : counter - 1,
        "suggestion": ' '.join(map(str, games[counter - 1].get_best_solution()[0]))
    })

# send user move
# in: game_id, action (add, remove), job_id, index
# out: suggestion


@app.route("/move/<game_id>", methods=["POST"])
def move(game_id):
    game_id = int(game_id)
    action = request.form['action']
    job_id = int(request.form['jobId'])
    index = int(request.form['index'])

    games[game_id].change_pheromone(job_id, index, True if action == "add" else False)
    games[game_id].run(iters=NUMBER_OF_ITERATIONS)

    return jsonify({
        "gameId": game_id,
        "suggestion": ' '.join(map(str, games[game_id].get_best_solution()[0]))
    })


if __name__ == '__main__':
    app.run()
