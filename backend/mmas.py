import numpy as np
import time

SEED = 42
INF = np.inf


class PermutationFlowShopScheduling:
    def __init__(self, number_of_machines=3, number_of_jobs=5, times=None):
        self.number_of_machines = number_of_machines
        self.number_of_jobs = number_of_jobs
        if times is None:
            self.times = np.random.rand(number_of_jobs, number_of_machines)
        else:
            self.times = times

    def evaluate_solution(self, solution):
        (number_of_jobs, number_of_machines) = self.times.shape
        makespan = 0
        times = np.zeros(number_of_machines)

        for i in range(number_of_jobs):
            for j in range(number_of_machines):
                if i == 0:
                    times[j] = self.times[solution[i],
                                          j] if j == 0 else times[j - 1] + self.times[solution[i], j]
                else:
                    if j == 0:
                        times[j] = times[j] + self.times[solution[i], j]
                    else:
                        times[j] = times[j] + self.times[solution[i], j] if times[j -
                                                                                  1] < times[j] else times[j - 1] + self.times[solution[i], j]

                makespan = max(makespan, times[j])

        return makespan


class MaxMinAntSystem:
    # https://www.researchgate.net/profile/Thomas-Stuetzle/publication/2593620_An_Ant_Approach_to_the_Flow_Shop_Problem/links/0046353a2c198330ee000000/An-Ant-Approach-to-the-Flow-Shop-Problem.pdf
    def __init__(self, pfss, number_of_ants=10, p0=0.9, min_max_ratio=5, persistence_rate=0.75, max_iter=100, max_stagnation=10):
        self.pfss = pfss
        self.number_of_ants = number_of_ants
        self.p0 = p0
        self.min_max_ratio = min_max_ratio
        self.persistence_rate = persistence_rate
        self.max_iter = max_iter
        self.max_stagnation = max_stagnation
        self.pheromone = np.zeros((pfss.number_of_jobs, pfss.number_of_jobs))
        self.best_solution = None
        self.best_evaluation = INF
        self.iter = 0
        self.stagnation_iter = 0
        self._init_pheromone()

    def get_best_solution(self):
        return self.best_solution, self.pfss.evaluate_solution(self.best_solution)

    def _init_pheromone(self):
        random_solution = np.arange(self.pfss.number_of_jobs)
        np.random.shuffle(random_solution)
        self._update_pheromone([random_solution], [
                               self.pfss.evaluate_solution(random_solution)])

    def _update_pheromone(self, solutions, evaluations):
        best_solution_index = np.argmax(evaluations)
        best_solution = solutions[best_solution_index]
        best_evaluation = evaluations[best_solution_index]

        self.pheromone = self.pheromone * self.persistence_rate
        self.pheromone[best_solution, np.arange(
            self.pfss.number_of_jobs)] += 1 / best_evaluation

        pheromone_max = 1 / ((1 - self.persistence_rate)
                             * min(self.best_evaluation, best_evaluation))
        pheromone_min = pheromone_max / self.min_max_ratio
        self.pheromone = np.maximum(
            pheromone_min, np.minimum(pheromone_max, self.pheromone))

        if best_evaluation < self.best_evaluation:
            self.best_evaluation = best_evaluation
            self.stagnation_iter = 0
        else:
            self.stagnation_iter += 1
            if self.stagnation_iter >= self.max_stagnation:
                self.stagnation_iter = 0
                self.best_evaluation = best_evaluation
                self._init_pheromone()

    def _construct_solution_from_pheromone(self):
        solution = np.zeros(self.pfss.number_of_jobs, dtype=np.int32)
        free_jobs = list(np.arange(self.pfss.number_of_jobs))

        for ind in range(self.pfss.number_of_jobs):
            if np.random.rand() < self.p0:
                job_index = np.argmax(self.pheromone[free_jobs, ind])
            else:
                probabilities = self.pheromone[free_jobs, ind]
                job_index = np.random.multinomial(
                    1, probabilities / np.sum(probabilities))
                job_index = np.nonzero(job_index)[0][0]
            solution[ind] = free_jobs[job_index]
            del free_jobs[job_index]

        return solution

    def run(self):
        best_evaluation = INF

        while True:
            solutions = []
            evaluations = []

            for _ in range(self.number_of_ants):
                solution = self._construct_solution_from_pheromone()
                evaluation = self.pfss.evaluate_solution(solution)

                if evaluation < best_evaluation:
                    self.best_solution = solution
                    best_evaluation = evaluation

                solutions.append(solution)
                evaluations.append(evaluation)

            self._update_pheromone(solutions, evaluations)

            self.iter += 1
            if self.iter > self.max_iter:
                break


if __name__ == "__main__":
    np.random.seed(SEED)

    pfss = PermutationFlowShopScheduling(
        number_of_machines=4, number_of_jobs=15)
    print(
        f"Problem representation\nnumber of machines: {pfss.number_of_machines}\nnumber of jobs: {pfss.number_of_jobs}\ntimes: {pfss.times}")

    start_time = time.time()

    mmas = MaxMinAntSystem(pfss)
    mmas.run()

    end_time = time.time()

    print(f"Best solution: {mmas.get_best_solution()}")
    print(f"Time elapsed: {end_time - start_time} s")
