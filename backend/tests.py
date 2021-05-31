from ast import literal_eval
from mmas import PermutationFlowShopScheduling, MaxMinAntSystem
import numpy as np
import os
import re
import time

REPETITIONS = 30
START_SEED = 23

if __name__ == "__main__":
    path = "./results"
    folders = os.listdir(path)

    for folder in folders:
        folder_path = f"{path}/{folder}"
        if os.path.isdir(folder_path) and not folder.startswith("."):
            file_path = f"{folder_path}/{folder}.data"
            with open(file_path, 'r') as f:
                number_of_jobs, number_of_machines = [int(x) for x in next(f).split()]
                times = ''.join(f.readlines())
                times = re.sub('\n', '', times)
                times = re.sub('\] \[', '],[', times)
                times = re.sub(r"([0-9]) +([0-9])", r"\1,\2", times)
                times = np.array(literal_eval(times))

            pfss = PermutationFlowShopScheduling(number_of_jobs=number_of_jobs, number_of_machines=number_of_machines, times=times)

            average_makespan = 0.0
            average_iter = 0
            average_time = 0.0

            start_time = time.time()

            for _ in range(REPETITIONS):
                np.random.seed(START_SEED)
                START_SEED += 10

                mmas = MaxMinAntSystem(pfss)
                mmas.run()

                solution = mmas.get_best_solution_result()
                average_makespan += mmas.get_best_makespan()
                average_iter += mmas.get_best_iter()

            end_time = time.time()

            average_makespan /= REPETITIONS
            average_iter = int(average_iter / REPETITIONS)
            average_time = (end_time - start_time) / REPETITIONS

            with open(f"{folder_path}/{folder}.mmas_multiple", 'w') as f:
                f.write(f"Solution: {solution}\n")
                f.write(f"Average makespan: {average_makespan}\n")
                f.write(f"Average iter: {average_iter}\n")
                f.write(f"Average time: {average_time}\n")

            print(f"{file_path} finished.")
