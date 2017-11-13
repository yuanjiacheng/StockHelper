using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Extension;
using Common.Helper;

namespace NeuralNetWork
{
    /// <summary>
    /// BPNN 网络，没有偏置属性，用偏置节点代替
    /// </summary>
    public class BPNN
    {
        /// <summary>
        /// 输入层输入
        /// </summary>
        public double[] i_input;
        /// <summary>
        /// 输出层输出
        /// </summary>
        public double[] i_output;
        /// <summary>
        /// 隐藏层输入
        /// </summary>
        public double[][] h_input;
        /// <summary>
        /// 隐藏层输出
        /// </summary>
        public double[][] h_output;
        /// <summary>
        /// 隐藏层权值
        /// </summary>
        public double[][,] h_weights;
        /// <summary>
        /// 输出层上一次梯度
        /// </summary>
        public double[][,] h_lastTheta;
        /// <summary>
        /// 输出层输入
        /// </summary>
        public double[] o_input;
        /// <summary>
        /// 输出层输出
        /// </summary>
        public double[] o_output;
        /// <summary>
        /// 输出层权值
        /// </summary>
        public double[,] o_weights;
        /// <summary>
        /// 输出层上一次梯度
        /// </summary>
        public double[,] o_lastTheta;
        /// <summary>
        /// 学习步长
        /// </summary>
        public double eta;
        /// <summary>
        /// 上一次训练步长
        /// </summary>
        public double lastEta;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="inputLayerNum">输入层神经元数量</param>
        /// <param name="hiddenLayerNum">隐藏层层数和神经元数量</param>
        /// <param name="outputLayersNum">输出层神经元数量</param>
        /// <param name="eta">学习步长</param>
        public BPNN()
        {

        }
        public BPNN(int inputLayerNum, int[] hiddenLayerNum, int outputLayersNum, double eta, double lastEta)
        {
            OtherHelper otherHelper = new OtherHelper();
            this.eta = eta;
            this.lastEta = lastEta;
            i_input = new double[inputLayerNum + 1];
            i_output = new double[inputLayerNum + 1];
            h_input = new double[hiddenLayerNum.Length][];
            h_output = new double[hiddenLayerNum.Length][];
            h_weights = new double[hiddenLayerNum.Length][,];
            h_lastTheta = new double[hiddenLayerNum.Length][,];
            for (int i = 0; i < hiddenLayerNum.Length; i++)
            {
                h_input[i] = new double[hiddenLayerNum[i] + 1];
                h_output[i] = new double[hiddenLayerNum[i] + 1];
                int row = 0;
                int col = 0;
                if (i == 0) //为零时时输入层
                {
                    row = inputLayerNum + 1;
                    col = hiddenLayerNum[i];
                }
                else
                {
                    row = hiddenLayerNum[i - 1] + 1;
                    col = hiddenLayerNum[i];
                }

                h_weights[i] = new double[row, col];
                h_lastTheta[i] = new double[row, col];
                for (int j = 0; j < row; j++)
                {
                    for (int k = 0; k < col; k++)
                    {
                        h_weights[i][j, k] = OtherHelper.getRandDouble(-200, 201);
                        h_lastTheta[i][j, k] = 0;
                    }
                }
            }
            o_input = new double[outputLayersNum];
            o_output = new double[outputLayersNum];
            o_weights = new double[hiddenLayerNum[hiddenLayerNum.Length - 1] + 1, outputLayersNum];
            o_lastTheta = new double[hiddenLayerNum[hiddenLayerNum.Length - 1] + 1, outputLayersNum];
            for (int i = 0; i < o_weights.GetLength(0); i++)
            {
                for (int j = 0; j < o_weights.GetLength(1); j++)
                {
                    o_weights[i, j] = OtherHelper.getRandDouble(-20, 20);
                    o_lastTheta[i, j] = 0;
                }
            }
        }
        /// <summary>
        /// 训练
        /// </summary>
        /// <param name="netWork"></param>
        /// <param name="inputs"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double train(ref BPNN netWork, double[][] inputs, double[][] targets)
        {
            double result = 0;
            for(int i=0;i<inputs.Length;i++)
            {
                computeForword(ref netWork, inputs[i]);
                double res= backPropagate(ref netWork, targets[i]);
                result += res;
            }
            //Console.WriteLine(j);
            return result / inputs.Length;
        }
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="netWork"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] forecast(ref BPNN netWork, double[] inputs)
        {
            return computeForword(ref netWork, inputs);
        }


        #region 私有方法
        /// <summary>
        /// 前向计算
        /// </summary>
        /// <param name="netWork">神经网络</param>
        /// <param name="inputs">输入值</param>
        /// <returns>输出值</returns>
        private double[] computeForword(ref BPNN network, double[] inputs)
        {
            #region 输入层赋值
            for (int i = 0; i < inputs.Length; i++)
            {
                network.i_input[i] = inputs[i];
                network.i_output[i] = network.i_input[i].Tanh();
            }
            network.i_input[network.i_input.Length - 1] = 1;
            network.i_output[network.i_output.Length - 1] = network.i_input[network.i_input.Length - 1].Tanh();
            #endregion
            #region 计算隐藏层
            double[] Oij = network.i_output;    //上一层输出
            for (int i = 0; i < network.h_input.Length; i++)
            {
                if (i > 0)
                {
                    Oij = network.h_output[i - 1];
                }
                for (int j = 0; j < network.h_input[i].Length - 1; j++)
                {
                    network.h_input[i][j] = getInputValue(network.h_weights[i], Oij, j);
                    network.h_output[i][j] = network.h_input[i][j].Tanh();
                }
                network.h_input[i][network.h_input[i].Length - 1] = 1;
                network.h_output[i][network.h_input[i].Length - 1] = (1.0).Tanh();
            }
            #endregion
            #region 计算输出层
            Oij = network.h_output[network.h_output.Length - 1];
            for (int i = 0; i < network.o_input.Length; i++)
            {
                network.o_input[i] = getInputValue(network.o_weights, Oij, i);
                network.o_output[i] = network.o_input[i].Tanh();
            }
            #endregion
            return network.o_output;
        }
        /// <summary>
        /// 向后传播，更新权值
        /// </summary>
        /// <param name="network"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private double backPropagate(ref BPNN network, double[] targets)
        {
            //int row = targets.GetLength(0);     //样本数量
            //int column = targets.GetLength(1);  //样本输出数量
            double[] oDelta = new double[targets.Length];
            #region 计算输出层Delta
            for (int i = 0; i < targets.Length; i++)
            {
                double error = targets[i] - network.o_output[i];
                oDelta[i] = error * (network.o_output[i].DTanh());
            }
            #endregion
            #region 计算隐藏层Delta
            double[][] hDeltas = new double[network.h_input.Length][];
            for (int i = hDeltas.Length - 1; i >= 0; i--)       //从最后一层开始
            {
                hDeltas[i] = new double[h_input[i].Length];
                for (int j = 0; j < hDeltas[i].Length; j++)   //j:当前层节点数量，包括了偏置节点
                {
                    double error = 0;
                    if (i == network.h_input.Length - 1)    //是最后一层隐藏层
                    {
                        for (int k = 0; k < oDelta.Length; k++)
                        {
                            error += oDelta[k] * network.o_weights[j, k];
                        }
                    }
                    else
                    {
                        for (int k = 0; k < network.h_output[i + 1].Length - 1; k++)    //k:后一层隐藏层节点数量，不包含偏置
                        {
                            error += hDeltas[i + 1][k] * network.h_weights[i + 1][j, k];
                        }
                    }
                    hDeltas[i][j] = error * (network.h_output[i][j].DTanh());
                }
            }
            #endregion
            #region 更新输出层权值 梯度=输出层Delta*隐藏层输出
            double[] lastHiddenOutput = network.h_output[network.h_output.Length - 1]; //最后一层隐藏层输出

            for (int j = 0; j < oDelta.Length; j++)
            {
                for (int i = 0; i < lastHiddenOutput.Length; i++) //i:最后一层隐藏层节点数，包括偏置节点
                {
                    double theta = 0;
                    theta += oDelta[j] * lastHiddenOutput[i];
                    network.o_weights[i, j] += theta * network.eta + network.o_lastTheta[i, j] * network.lastEta;
                    network.o_lastTheta[i, j] = theta;
                }
            }
            #endregion
            #region 更新隐藏层权值
            for (int i = hDeltas.Length - 1; i >= 0; i--)   //从最后一层隐藏层开始更新
            {
                int lastNeuralCount = i == 0 ? network.i_output.Length : network.h_output[i - 1].Length;    //上一层节点数，包含偏置
                for (int j = 0; j < hDeltas[i].Length - 1; j++)   //j:当前隐藏层节点数，不包含偏置
                {
                    double theta = 0;
                    for (int k = 0; k < lastNeuralCount; k++)
                    {
                        if (i == 0)
                        {
                            theta += hDeltas[i][j] * network.i_output[k];
                        }
                        else
                        {
                            theta += hDeltas[i][j] * network.h_output[i - 1][k];
                        }
                        network.h_weights[i][k, j] += network.eta * theta + network.lastEta * network.h_lastTheta[i][k, j];
                        network.h_lastTheta[i][k, j] = theta;
                    }
                }
            }
            #endregion
            double result = 0.0;
         
                for (int i = 0; i < targets.Length; i++)
                {
                    result += Math.Pow(targets[i] - network.o_output[i],2);
                }
            return result / targets.Length;
        }

        /// <summary>
        /// 计算一个神经元获得的输入量；公式：sum(Wij*Opj)+Bi
        /// </summary>
        /// <param name="Wij">权重</param>
        /// <param name="Opj">值</param>
        /// <param name="j">列</param>
        /// <returns>输入量</returns>
        private double getInputValue(double[,] Wij, double[] Opj, int j)
        {
            double result = 0;
            for (int i = 0; i < Opj.Length; i++)
            {
                result += Wij[i, j] * Opj[i];
            }
            return result;
        }
        #endregion


    }
}
