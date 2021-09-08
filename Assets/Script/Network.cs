using System;

public class Network
{
    public int inp_neuron { get; private set; }
    public int hid_neuron { get; private set; }
    public int out_neuron { get; private set; }
 
    public Matrix W0 { get; private set; }
    public Matrix W1 { get; private set; }
    public Matrix B1 { get; private set; }
    public Matrix B2 { get; private set; }

    public Network(int inp_neuron, int hid_neuron, int out_neuron)
    {
        this.inp_neuron = inp_neuron;
        this.hid_neuron = hid_neuron;
        this.out_neuron = out_neuron;
        W0 = Matrix.random(hid_neuron, inp_neuron);
        W1 = Matrix.random(out_neuron, hid_neuron);
        B1 = Matrix.random(hid_neuron, 1);
        B2 = Matrix.random(out_neuron, 1);
    }

    public Network(Matrix w0, Matrix w1, Matrix b1, Matrix b2)
    {
        if (w0.RowCount() != w1.ColCount())      { throw new DataMisalignedException("W0 Row Count (" + w0.RowCount() + ") != W1 Column Count (" + w1.ColCount() + ")"); }
        else if (b1.RowCount() != w0.RowCount()) { throw new DataMisalignedException("B1 Row Count (" + b1.RowCount() + ") != W0 Row Count (" + w0.RowCount() + ")"); }
        else if (b2.RowCount() != w1.RowCount()) { throw new DataMisalignedException("B2 Row Count (" + b2.RowCount() + ") != W1 Row Count (" + w1.RowCount() + ")"); }
        else if (b1.ColCount() != 1)             { throw new DataMisalignedException("B1 Column Count (" + b1.ColCount() + ") != 1"); }
        else if (b2.ColCount() != 1)             { throw new DataMisalignedException("B2 Column Count (" + b2.ColCount() + ") != 1"); }

        inp_neuron = w0.ColCount();
        hid_neuron = w0.RowCount();
        out_neuron = w1.RowCount();
        W0 = w0;
        W1 = w1;
        B1 = b1;
        B2 = b2;
    }

    public static string GetNetSpecs(Network network)
    {
        return "(" + network.inp_neuron + ", " + network.hid_neuron + ", " + network.out_neuron + ")";
    }

    public string GetSpecs() { return GetNetSpecs(this); }

    public Matrix Predict(Matrix inp)
    {
        return B2 + (W1 * Acts.Tanh(W0 * inp + B1));
    }

    public Network MutatedCopy(float mutationRate)
    {
        return new Network(W0.mutatedCopy(mutationRate), W1.mutatedCopy(mutationRate), 
                           B1.mutatedCopy(mutationRate), B2.mutatedCopy(mutationRate));
    }
}

