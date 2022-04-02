using Tensorflow;
using Tensorflow.Keras.Losses;
using Tensorflow.Keras.Utils;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace ML.Handwritten.GAN
{
    public class BinaryCrossentropy : LossFunctionWrapper, ILossFunc
    {
        public BinaryCrossentropy(string reduction = null, string name = null)
           : base(reduction, (name == null) ? "BinaryCrossentropy" : name)
        {
        }

        public override Tensor Apply(Tensor y_true = null, Tensor y_pred = null, bool from_logits = false, int axis = -1)
        {
            y_pred = ops.convert_to_tensor(y_pred);
            y_true = gen_math_ops.cast(y_true, y_pred.dtype);
            if (from_logits)
            {
                y_pred = gen_math_ops.sigmoid(y_pred);
            }
            var epsilon = constant_op.constant(keras.backend.epsilon(), y_pred.dtype.as_base_dtype());
            y_pred = clip_ops.clip_by_value(y_pred, epsilon, 1f - epsilon);

            return -gen_math_ops.mean(y_true * gen_math_ops.log(y_pred) + (1 - y_true) * gen_math_ops.log(1 - y_pred), -1);
            /*
            y_pred = ops.convert_to_tensor(y_pred);
            CheckNonValue(y_pred, "y_pred");
            y_true = gen_math_ops.cast(y_true, y_pred.dtype);
            CheckNonValue(y_true, "y_true");
            if (from_logits)
            {
                y_pred = gen_math_ops.sigmoid(y_pred);
                CheckNonValue(y_pred, "y_pred");
            }

            var epsilon = constant_op.constant(keras.backend.epsilon(), y_pred.dtype.as_base_dtype());
            y_pred = clip_ops.clip_by_value(y_pred, epsilon, 1f - epsilon);
            CheckNonValue(y_pred, "y_pred");

            var oneMinusLabel = 1 - y_true;
            CheckNonValue(oneMinusLabel, "oneMinusLabel");
            var oneMinusPred = 1 - y_pred;
            CheckNonValue(oneMinusPred, "oneMinusPred");
            var logPred = gen_math_ops.log(y_pred);
            CheckNonValue(logPred, "logPred");
            var logOneMinusPred = gen_math_ops.log(oneMinusPred);

            var y_trueMullogPred = y_true * logPred;
            var oneMinusLabelMullogOneMinusPred = oneMinusLabel * logOneMinusPred;

            CheckNonValue(logOneMinusPred, "logOneMinusPred");
            CheckNonValue(y_trueMullogPred, "y_trueMullogPred");
            if (!CheckNonValue(oneMinusLabelMullogOneMinusPred, "oneMinusLabelMullogOneMinusPred"))
            {
                var index = FindNanIndex(oneMinusLabelMullogOneMinusPred);
                UnityEngine.Debug.LogError($"log({oneMinusPred[index].numpy()}) = {gen_math_ops.log(oneMinusPred[index]).numpy()}");
                UnityEngine.Debug.LogError($"{oneMinusLabel[index].numpy()} * {logOneMinusPred[index].numpy()} = {oneMinusLabel[index] * logOneMinusPred[index]}");
            }
            var v = y_trueMullogPred + oneMinusLabelMullogOneMinusPred;
            CheckNonValue(v, "v");
            var mean = gen_math_ops.mean(v, -1);
            CheckNonValue(mean, "mean");
            var value = -mean;
            CheckNonValue(value, "value");
            var sample_weight = ((value.dtype == TF_DataType.TF_DOUBLE) ? Binding.tf.constant(1.0) : Binding.tf.constant(1f));
            var scale_losses_by_sample_weight = losses_utils.scale_losses_by_sample_weight(value, sample_weight);
            CheckNonValue(scale_losses_by_sample_weight, "scale_losses_by_sample_weight");
            var loss = reduce_weighted_loss(scale_losses_by_sample_weight, "sum_over_batch_size");
            CheckNonValue(loss, "loss");
            return value;
            */
        }

        private bool CheckNonValue(Tensor tensor, string name)
        {
            if (Utility.Math.MathUtility.IsNaN(tensor))
            {
                UnityEngine.Debug.Log($"{name}: {tensor}");
                UnityEditor.EditorApplication.isPlaying = false;
                return false;
            }
            return true;
        }
        private int FindNanIndex(Tensor tensor)
        {
            for (int i = 0; i < tensor.shape[0]; i++)
            {
                if (Utility.Math.MathUtility.IsNaN(tensor[i]))
                {
                    return i;
                }
            }
            throw new System.Exception("Not Found");
        }

        private Tensor reduce_weighted_loss(Tensor weighted_losses, string reduction)
        {
            if (reduction == "none")
            {
                return weighted_losses;
            }

            Tensor tensor = math_ops.reduce_sum(weighted_losses);
            CheckNonValue(tensor, "tensor");
            if (reduction == "sum_over_batch_size")
            {
                tensor = _safe_mean(tensor, _num_elements(weighted_losses));
                CheckNonValue(tensor, "tensor");
            }

            return tensor;
        }

        private Tensor _safe_mean(Tensor losses, Tensor num_present)
        {
            var v = math_ops.div_no_nan(math_ops.reduce_sum(losses), num_present, "value");
            CheckNonValue(v, "v");
            return v;
        }

        private Tensor _num_elements(Tensor losses)
        {
            Tensor losses2 = losses;
            var v = Binding.tf_with(ops.name_scope("num_elements"), (ops.NameScope scope) => math_ops.cast(array_ops.size(losses2, scope), losses2.dtype));
            CheckNonValue(v, "v");
            return v;
        }
    }
}