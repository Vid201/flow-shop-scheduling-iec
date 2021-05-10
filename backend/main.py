from flask import Flask, request, jsonify
from mmas import PermutationFlowShopScheduling, MaxMinAntSystem
import numpy as np

app = Flask(__name__)

counter = 0
games = {}


# send game data at the beginning
# in: game data (number of jobs, number of machines, jobs data)
# out: game_id


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
    
    return jsonify({
        "gameId" : counter - 1
    })

# send user move
# in: game_id, move
# out: suggestions


@app.route("/move/<game_id>", methods=["POST"])
def move(game_id):
    pass


if __name__ == '__main__':
    app.run()
