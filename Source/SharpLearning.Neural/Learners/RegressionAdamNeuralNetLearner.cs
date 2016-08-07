﻿using SharpLearning.Common.Interfaces;
using SharpLearning.Containers.Matrices;
using SharpLearning.Neural.Activations;
using SharpLearning.Neural.Layers;
using SharpLearning.Neural.Loss;
using SharpLearning.Neural.Models;
using SharpLearning.Neural.Optimizers;
using SharpLearning.Neural.TargetEncoders;
using System;

namespace SharpLearning.Neural.Learners
{
    /// <summary>
    /// RegressionNeuralNet learner utilizing the Adam method for stochastic optimization. 
    /// is well suited for problems that are large in terms of data and/or parameters.
    /// The method is also appropriate for non-stationary objectives and problems with
    /// very noisy and/or sparse gradients. https://arxiv.org/pdf/1412.6980.pdf
    /// </summary>
    public sealed class RegressionAdamNeuralNetLearner : NeuralNetLearner, IIndexedLearner<double>, ILearner<double>
    {
        /// <summary>
        /// RegressionNeuralNet learner utilizing the Adam method for stochastic optimization. 
        /// is well suited for problems that are large in terms of data and/or parameters.
        /// The method is also appropriate for non-stationary objectives and problems with
        /// very noisy and/or sparse gradients. https://arxiv.org/pdf/1412.6980.pdf
        /// </summary>
        public RegressionAdamNeuralNetLearner()
            : this(new HiddenLayer[] { HiddenLayer.New(100) }, new ReluActivation(), new SquaredLoss())
        {
        }

        /// <summary>
        /// RegressionNeuralNet learner utilizing the Adam method for stochastic optimization. 
        /// is well suited for problems that are large in terms of data and/or parameters.
        /// The method is also appropriate for non-stationary objectives and problems with
        /// very noisy and/or sparse gradients. https://arxiv.org/pdf/1412.6980.pdf
        /// </summary>
        /// <param name="hiddenLayers">Hidden layers. The layers is initializes in the order they appear in the array</param>
        public RegressionAdamNeuralNetLearner(HiddenLayer[] hiddenLayers)
            : this(hiddenLayers, new ReluActivation(), new SquaredLoss())
        {
        }

        /// <summary>
        /// RegressionNeuralNet learner utilizing the Adam method for stochastic optimization. 
        /// is well suited for problems that are large in terms of data and/or parameters.
        /// The method is also appropriate for non-stationary objectives and problems with
        /// very noisy and/or sparse gradients. https://arxiv.org/pdf/1412.6980.pdf
        /// </summary>
        /// <param name="hiddenLayers">Hidden layers. The layers is initializes in the order they appear in the array.
        /// (default is single layer with 100 units)</param>
        /// <param name="activiation">The type of activation used in the hidden layers (Default is Rectifier linear units)</param>
        public RegressionAdamNeuralNetLearner(HiddenLayer[] hiddenLayers, IActivation activiation)
            : this(hiddenLayers, activiation, new SquaredLoss())
        {
        }

        /// <summary>
        /// RegressionNeuralNet learner utilizing the Adam method for stochastic optimization. 
        /// is well suited for problems that are large in terms of data and/or parameters.
        /// The method is also appropriate for non-stationary objectives and problems with
        /// very noisy and/or sparse gradients. https://arxiv.org/pdf/1412.6980.pdf
        /// </summary>
        /// <param name="hiddenLayers">Hidden layers. The layers is initializes in the order they appear in the array 
        /// (default is single layer with 100 units)</param>
        /// <param name="activiation">The type of activation used in the hidden layers (Default is Rectifier linear units)</param>
        /// <param name="loss">Loss to minimize (Default is square loss)</param>
        /// <param name="maxIterations">The maximum number of iterations before termination. (Default is 100)</param>
        /// <param name="learningRateInitial">Initial learning rate. Controls the step size when updating the weights. (Default is 0.001)</param>
        /// <param name="batchSize">Batch size for mini-batch stochastic gradient descent.(Default is 200)</param>
        /// <param name="l2regularization">L2 reguralization term. (Default is 0.0001)</param>
        /// <param name="inputDropOut">Input dropout percentage. The percentage of units randomly omitted during training.
        /// This is a reguralizatin methods for reducing overfitting. Recommended value is 0.1 and range should be between 0.0 and 0.3. Default is (0.0)
        /// https://www.cs.toronto.edu/~hinton/absps/JMLRdropout.pdf</param>
        /// <param name="beta1">Exponential decay rate for estimates of first moment vector, should be in range 0 to 1</param>
        /// <param name="beta2">Exponential decay rate for estimates of second moment vector, should be in range 0 to 1</param>
        /// <param name="shuffle">Decides if the observations should be shuffled between each iteration. (Default is true)</param>
        /// <param name="seed">Seed for random initialization of weights. (defualt is 42)</param>
        /// <param name="tol">Tolerence for the optimization. If the training loss has not improved be tol 
        /// in two consequitive iterations. The optimization terminates. (Default is 0.0001)</param>
        public RegressionAdamNeuralNetLearner(HiddenLayer[] hiddenLayers, IActivation activiation, ILoss loss, int maxIterations = 200, double learningRateInitial = 0.001,
            int batchSize = 100, double l2regularization = 0.0001, double inputDropOut = 0.0, double beta1 = 0.9, double beta2 = 0.999, bool shuffle = true, int seed = 42, double tol = 1e-4)
            : base(hiddenLayers, activiation, loss, new CopyTargetEncoder(), new IdentityActivation(),
                new AdamNeuralNetOptimizer(learningRateInitial, beta1, beta2), maxIterations,
                batchSize, l2regularization, inputDropOut, shuffle, seed, tol)
        {
        }

        /// <summary>
        /// Learns a regression neural network
        /// </summary>
        /// <param name="observations"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public RegressionNeuralNetModel Learn(F64Matrix observations, double[] targets)
        {
            var model = this.BaseLearn(observations, targets, null, null, 0, null);
            return new RegressionNeuralNetModel(model);
        }

        /// <summary>
        /// Learns a regression neural network
        /// </summary>
        /// <param name="observations"></param>
        /// <param name="targets"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public RegressionNeuralNetModel Learn(F64Matrix observations, double[] targets, int[] indices)
        {
            var model = this.BaseLearn(observations, targets, indices, null, null, 0, null);
            return new RegressionNeuralNetModel(model);
        }

        /// <summary>
        /// Learns a RegressionNeuralNetModel with early stopping.
        /// The parameter earlyStoppingRounds controls how often the validation error is measured.
        /// If the validation error has increased, the learning is stopped and the model from the current iteration is returned.
        /// </summary>
        /// <param name="observations"></param>
        /// <param name="targets"></param>
        /// <param name="validationObservations"></param>
        /// <param name="validationTargets"></param>
        /// <param name="metric">Metric used for measuring the validation error</param>
        /// <param name="earlyStoppingRounds">How often is the validation error measured</param>
        /// <returns></returns>
        public RegressionNeuralNetModel LearnWithEarlyStopping(F64Matrix observations, double[] targets,
                F64Matrix validationObservations, double[] validationTargets, IMetric<double, double> metric,
                int earlyStoppingRounds)
        {
            Func<F64Matrix, double[], double> earlyStopping = (valObs, valTargets) =>
            {
                var validationModel = new RegressionNeuralNetModel(
                    new NeuralNetModel(m_coefs, m_intercepts,
                    m_hiddenActiviationFunc(), m_outputActiviationFunc(), 0));

                var validationPredictions = validationModel.Predict(valObs);
                var validationError = metric.Error(valTargets, validationPredictions);
                return validationError;
            };

            var model = this.BaseLearn(observations, targets, validationObservations, validationTargets,
                earlyStoppingRounds, earlyStopping);

            return new RegressionNeuralNetModel(model);
        }

        /// <summary>
        /// Learns a regression neural network
        /// </summary>
        /// <param name="observations"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        IPredictorModel<double> ILearner<double>.Learn(F64Matrix observations, double[] targets)
        {
            return Learn(observations, targets);
        }

        /// <summary>
        /// Learns a regression neural network
        /// </summary>
        /// <param name="observations"></param>
        /// <param name="targets"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        IPredictorModel<double> IIndexedLearner<double>.Learn(F64Matrix observations, double[] targets, int[] indices)
        {
            return Learn(observations, targets, indices);
        }
    }
}