using UnityEngine;
using System;


public class NetworkVisual: MonoBehaviour{
    [HideInInspector]
    private Network drawingNet;

    [Header("Specifications")]
    public int inpNeuronsCount;
    public int hidNeuronsCount;
    public int outNeuronsCount;

    [Header("Referances")]
    public Material material;
    public GameObject neuronPrefab;
    public Color negativeColor;
    public Color positiveColor;

    [Header("Positioning")]
    public float neuronOffset;
    public float layerOffset;

    LineRenderer[,] linesIH;
    LineRenderer[,] linesHO;
    Vector2 centerPoint;

    private void Awake()
    {
        centerPoint = new Vector2(transform.position.x, transform.position.y);
        linesIH = new LineRenderer[hidNeuronsCount, inpNeuronsCount];
        linesHO = new LineRenderer[outNeuronsCount, hidNeuronsCount];
        DrawLines();
        DrawPoints();
    }

    private void DrawPoints() {
        Vector2 pos = centerPoint + new Vector2(-layerOffset, (inpNeuronsCount-1)*neuronOffset/2);

        // Input Layer
        for (int ic=0; ic < inpNeuronsCount; ic++) {
            
            Instantiate(neuronPrefab, pos, Quaternion.identity, transform).name = "Neuron I"+ic;
            pos.y -= neuronOffset; }

        // Hidden Layer
        pos.x += layerOffset;
        pos.y = centerPoint.y + (hidNeuronsCount - 1) * neuronOffset / 2;
        for (int hc = 0; hc < hidNeuronsCount; hc++) {
            Instantiate(neuronPrefab, pos, Quaternion.identity, transform).name = "Neuron H" + hc;
            pos.y -= neuronOffset;}

        // Output Layer
        pos.x += layerOffset;
        pos.y = centerPoint.y + (outNeuronsCount - 1) * neuronOffset / 2;
        for (int oc = 0; oc < outNeuronsCount; oc++) {
            Instantiate(neuronPrefab, pos, Quaternion.identity, transform).name = "Neuron O" + oc;
            pos.y -= neuronOffset;} }

    private void DrawLines() {
        Vector2 lineStartPoint = centerPoint + new Vector2(-layerOffset, (inpNeuronsCount - 1) * neuronOffset / 2);
        Vector2 lineEndPoint = centerPoint + new Vector2(0, (hidNeuronsCount - 1) * neuronOffset / 2);
        
        for (int i = 0; i < inpNeuronsCount; i++) {
            for (int h = 0; h < hidNeuronsCount; h++) {
                GameObject go = new GameObject("Weight I" + i + " to H" + h);
                LineRenderer lr = go.AddComponent<LineRenderer>();
                go.transform.SetParent(transform);

                lr.SetPosition(0, lineStartPoint);
                lr.SetPosition(1, lineEndPoint);
                lr.material = material;
                linesIH[h, i] = lr;
                lineEndPoint.y -= neuronOffset;
                EditLine(lr, positiveColor, 1f);
            }
            lineEndPoint.y = centerPoint.y + (hidNeuronsCount - 1) * neuronOffset / 2;
            lineStartPoint.y -= neuronOffset;
        }

        // 
        lineStartPoint.x += layerOffset;
        lineStartPoint.y = centerPoint.y + (hidNeuronsCount - 1) * neuronOffset / 2;
        lineEndPoint.x += layerOffset;
        lineEndPoint.y = centerPoint.y + (outNeuronsCount - 1) * neuronOffset / 2;
        for (int h = 0; h < hidNeuronsCount; h++) {
            for (int o = 0; o < outNeuronsCount; o++) {
                GameObject go = new GameObject("Weight H" + h + " to O" + o);
                LineRenderer lr = go.AddComponent<LineRenderer>();
                go.transform.SetParent(transform);
                lr.material = material;
                lr.SetPosition(0, lineStartPoint);
                lr.SetPosition(1, lineEndPoint);
                lr.material = material;
                linesHO[o, h] = lr;
                lineEndPoint.y -= neuronOffset;
                EditLine(lr, positiveColor, 1f);
            }
            lineEndPoint.y = centerPoint.y + (outNeuronsCount - 1) * neuronOffset / 2;
            lineStartPoint.y -= neuronOffset;
        }
    }

    private void UpdateWeights() {
        for (int i = 0; i < inpNeuronsCount; i++) {
            for (int h = 0; h < hidNeuronsCount; h++) {
                if (drawingNet.W0[h, i] < 0) { EditLine(linesIH[h, i], negativeColor, -drawingNet.W0[h, i]); }
                else { EditLine(linesIH[h, i], positiveColor, drawingNet.W0[h, i]); }
            }
        }


        for (int h = 0; h < hidNeuronsCount; h++) {
            for (int o = 0; o < outNeuronsCount; o++) {
                if (drawingNet.W1[o, h] < 0) { EditLine(linesHO[o, h], negativeColor, -drawingNet.W1[o, h]); }
                else { EditLine(linesHO[o, h], positiveColor, drawingNet.W1[o, h]); }
            }
        }
    }

    private void EditLine(LineRenderer lr, Color color, float width)
    {
        lr.startColor = lr.endColor = color;
        lr.startWidth = lr.endWidth = width * 0.03f ;
    }

    public Network getNet() { return drawingNet; }

    public void setNet(Network newNet) {
        if (newNet.inp_neuron != inpNeuronsCount || newNet.hid_neuron != hidNeuronsCount || newNet.out_neuron != outNeuronsCount)
        {
            throw new DataMisalignedException("Provided netowrk doesn`t match expected neuron count:\n" +
                "Expected: " + drawingNet.GetSpecs() + "  Got: " + newNet.GetSpecs());
        }
        drawingNet = newNet;
        UpdateWeights();
    }
}
