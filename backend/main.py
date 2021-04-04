from flask import Flask, request

app = Flask(__name__)

# send game data at the beginning
# in: game data (number of jobs, number of machines, jobs data)
# out: game_id


@app.route("/game", methods=["POST"])
def game():
    pass

# send user move
# in: game_id, move
# out: suggestions


@app.route("/move/<game_id>", methods=["POST"])
def move(game_id):
    pass


if __name__ == '__main__':
    app.run()
