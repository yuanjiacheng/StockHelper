using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Extension;

namespace NeuralNetWork
{

    //第一步：随机初始化权值，偏置（ps:这里没有加偏置项，通过在输入层增加偏置节点代替）
    //第二步：前向计算
    //第三步：求前向计算输出层输出模型和目标模型的误差
    //第四步：后向传播，计算误差模型的梯度
    //第五步：更新权值，偏置
    public class BP
    {
        #region 类
        /// <summary>
        /// 神经网络
        /// </summary>
        public class NetWork
        {
            /// <summary>
            /// 输入层
            /// </summary>
            public Layer inputLayer;
            /// <summary>
            /// 隐藏层
            /// </summary>
            public Layer[] hiddenLayers;
            /// <summary>
            /// 输出层
            /// </summary>
            public Layer outputLayers;
            /// <summary>
            /// 学习速率
            /// </summary>
            public double Eta;
        }
        /// <summary>
        /// 神经层
        /// </summary>
        public class Layer
        {
            /// <summary>
            /// 包含的神经元
            /// </summary>
            public Neural[] Neurals;
        }
        /// <summary>
        /// 神经元，没有偏置，但在输入层添加一个偏置节点来实现偏置
        /// </summary>
        public class Neural
        {
            /// <summary>
            /// 与上一层的连接权重,输入层为null
            /// </summary>
            public double[] Weights;
            /// <summary>
            /// 输入量
            /// </summary>
            public double input;
            /// <summary>
            /// 输出量
            /// </summary>
            public double output;
        }
        #endregion

        #region 方法
        public NetWork initNetWork(string filePath)
        {
            NetWork result = new NetWork();
            return result;
        }
        /// <summary>
        /// 初始化bp网络
        /// </summary>
        /// <param name="inputLayerNum">输入测神经元数量</param>
        /// <param name="hiddenLayerNum">隐藏层神经元数量</param>
        /// <param name="outputLayersNum">输出层神经元数量</param>
        /// <param name="eta">学习速率</param>
        /// <returns></returns>
        public NetWork initNetWork(int inputLayerNum, int[] hiddenLayerNum, int outputLayersNum, double eta)
        {

            NetWork result = new NetWork();
            //加一个偏置节点
            inputLayerNum += 1;
            result.Eta = eta;
            result.inputLayer = new Layer() { Neurals = new Neural[inputLayerNum] };
            for (int i = 0; i < inputLayerNum; i++)
            {
                result.inputLayer.Neurals[i] = new Neural() { };
            }
            #region 初始化隐藏层
            result.hiddenLayers = new Layer[hiddenLayerNum.Length];
            int lastLayerNeuralCount = inputLayerNum;
            for (int i = 0; i < result.hiddenLayers.Length; i++)
            {
                result.hiddenLayers[i] = new Layer() { Neurals = new Neural[hiddenLayerNum[i]] };
                for (int j = 0; j < result.hiddenLayers[i].Neurals.Length; j++)
                {
                    result.hiddenLayers[i].Neurals[j] = new Neural() { Weights = new double[lastLayerNeuralCount] };
                    for (int k = 0; k < result.hiddenLayers[i].Neurals[j].Weights.Length; k++)
                    {
                        result.hiddenLayers[i].Neurals[j].Weights[k] = getRand(-100, 100);
                    }
                }
                lastLayerNeuralCount = hiddenLayerNum[i];
            }
            #endregion
            #region
            result.outputLayers = new Layer() { Neurals = new Neural[outputLayersNum] };
            for (int i = 0; i < outputLayersNum; i++)
            {
                result.outputLayers.Neurals[i] = new Neural() { Weights = new double[hiddenLayerNum[hiddenLayerNum.Length - 1]] };
                for (int j = 0; j < hiddenLayerNum[hiddenLayerNum.Length - 1]; j++)
                {
                    result.outputLayers.Neurals[i].Weights[j] = getRand(-20, 20);
                }
            }
            #endregion
            return result;
        }
        /// <summary>
        /// 训练bp网络
        /// </summary>
        /// <param name="netWork">神经网络</param>
        /// <param name="inputs">输入样本</param>
        /// <param name="targets">目标模型</param>
        /// <returns>输出误差</returns>
        public double train(ref NetWork netWork, double[] inputs, double[] targets)
        {
            computeForword(inputs, ref netWork.inputLayer, ref netWork.hiddenLayers, ref netWork.outputLayers);
            double result = backPropagate(ref netWork.inputLayer, ref netWork.hiddenLayers, ref netWork.outputLayers, netWork.Eta, targets);
            return result;
        }
        /// <summary>
        /// 使用神经网络预测
        /// </summary>
        /// <param name="netWork">训练好的bp网络</param>
        /// <param name="inputs">输入样本</param>
        /// <returns>预测结果</returns>
        public double[] forecast(NetWork netWork, double[] inputs)
        {
            return computeForword(inputs, ref netWork.inputLayer, ref netWork.hiddenLayers, ref netWork.outputLayers);
        }
        #region 私有方法
        private Random rand = new Random();
        /// <summary>
        /// 后向传播，更新权值
        /// </summary>
        /// <param name="inputLayer">输入层</param>
        /// <param name="hiddenLayers">隐藏层</param>
        /// <param name="outputLayer">输出层</param>
        /// <param name="eta">学习速率</param>
        /// <param name="targets">目标模型</param>
        /// <returns>误差</returns>
        private double backPropagate(ref Layer inputLayer, ref Layer[] hiddenLayers, ref Layer outputLayer, double eta, double[] targets)
        {
            double[] outputDelta = getOutputDelta(outputLayer.Neurals.Select(s => s.output).ToArray(), targets);
            updateWeights(ref outputLayer.Neurals, hiddenLayers[hiddenLayers.Length - 1].Neurals.Select(s => s.output).ToArray(), outputDelta, eta);
            double[] hiddenDelta = outputDelta;
            for (int i = hiddenLayers.Length - 1; i >= 0; i--)
            {
                hiddenDelta = getHiddenDelta(hiddenLayers[i].Neurals, hiddenDelta);
                if (i == 0)
                {
                    updateWeights(ref hiddenLayers[i].Neurals, inputLayer.Neurals.Select(s => s.output).ToArray(), hiddenDelta, eta);
                }
                else
                {
                    updateWeights(ref hiddenLayers[i].Neurals, hiddenLayers[i].Neurals.Select(s => s.output).ToArray(), hiddenDelta, eta);
                }

            }
            double result = 0.0;
            for (int i = 0; i < outputLayer.Neurals.Length; i++)
            {
                result += Math.Pow(targets[i] - outputLayer.Neurals[i].output, 2);
            }
            return result;
        }
        /// <summary>
        /// 前向计算
        /// </summary>
        /// <param name="inputs">输入数据</param>
        /// <param name="inputLayer">输入层</param>
        /// <param name="hiddenLayers">隐藏层</param>
        /// <param name="outputLayer">输出层</param>
        /// <returns>输出层输出</returns>
        private double[] computeForword(double[] inputs, ref Layer inputLayer, ref Layer[] hiddenLayers, ref Layer outputLayer)
        {
            #region 输入层赋值
            for (int i = 0; i < inputLayer.Neurals.Length; i++)
            {
                if (i < inputLayer.Neurals.Length - 1)
                {
                    inputLayer.Neurals[i].input = inputs[i];
                    inputLayer.Neurals[i].output = inputs[i];
                }
                else
                {
                    inputLayer.Neurals[i].input = 1;
                    inputLayer.Neurals[i].output = 1;
                }

            }
            #endregion
            #region 计算隐藏层输入输出
            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                double[] hInputs;
                if (i == 0)
                {
                    hInputs = inputLayer.Neurals.Select(s => s.output).ToArray();
                }
                else
                {
                    hInputs = hiddenLayers[i - 1].Neurals.Select(s => s.output).ToArray();
                }
                for (int j = 0; j < hiddenLayers[i].Neurals.Length; j++)
                {
                    var item = hiddenLayers[i].Neurals[j];
                    item.input = getInputValue(item.Weights, hInputs);
                    item.output = item.input.Tanh();
                }
            }
            #endregion
            #region 计算输出层输出
            var oInputs = hiddenLayers[hiddenLayers.Length - 1].Neurals.Select(s => s.output).ToArray();
            for (int i = 0; i < outputLayer.Neurals.Length; i++)
            {
                var item = outputLayer.Neurals[i];
                item.input = getInputValue(item.Weights, oInputs);
                item.output = item.input.Tanh();
            }
            #endregion
            return outputLayer.Neurals.Select(s => s.output).ToArray();
        }
        /// <summary>
        /// 计算一个神经元获得的输入量；公式：sum(Wij*Opj)+Bi
        /// </summary>
        /// <param name="Wij">权重</param>
        /// <param name="Opj">值</param>
        /// <returns>输入量</returns>
        private double getInputValue(double[] Wij, double[] Opj)
        {
            double result = 0;
            for (int i = 0; i < Wij.Length; i++)
            {
                result += Wij[i] * Opj[i];
            }
            return result;
        }
        /// <summary>
        /// 计算输出层的delta 
        /// </summary>
        /// <param name="outputs"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private double[] getOutputDelta(double[] outputs, double[] targets)
        {
            double[] result = new double[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                var error = targets[i] - outputs[i];
                result[i] = error * outputs[i].DTanh();
            }
            return result;
        }
        /// <summary>
        /// 计算隐藏层的Delta 
        /// </summary>
        /// <param name="outputs">输出量</param>
        /// <param name="delta">下一层计算出的delta</param>
        /// <param name="weights">该层的权值</param>
        /// <returns></returns>
        private double[] getHiddenDelta(Neural[] neurals, double[] delta)
        {
            double[] result = new double[neurals.Length];
            for (int i = 0; i < neurals.Length; i++)
            {
                double error = 0.0;
                for (int j = 0; j < delta.Length; j++)
                {
                    error += delta[j] * neurals[i].Weights[j];
                }
                result[i] = error * neurals[i].output.DTanh();
            }
            return result;
        }
        /// <summary>
        /// 更新权值 梯度=delta*权值
        /// </summary>
        /// <param name="neurals">要更新的权值的神经元</param>
        /// <param name="deltas">delta</param>
        /// <param name="eta">学习率</param>
        private void updateWeights(ref Neural[] neurals, double[] outputs, double[] deltas, double eta)
        {
            for (int i = 0; i < neurals.Length; i++)
            {
                for (int j = 0; j < deltas.Length; j++)
                {
                    neurals[i].Weights[j] += outputs[j] * deltas[j] * eta;
                }
            }
        }

        private double getRand(int min, int max)
        {
            double result = ((double)rand.Next(min, max)) / 100.00;
            if (result == 0)
            {
                result = getRand(min, max);
            }
            return result;
        }
        #endregion
        #endregion
    }
}
