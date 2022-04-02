using Tensorflow;
using Tensorflow.Keras.Losses;
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
        }
    }
}