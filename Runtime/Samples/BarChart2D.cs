using UnityEngine;
using UnityD3;
using System.Collections.Generic;

public class BarChart2D : MonoBehaviour
{
    private struct Record
    {
        public Record(float value)
        {
            Value = value;
        }
        public float Value;
    }

    ///////////////////////////
    /// SCALES
    ///////////////////////////
    private IScaleContinuous<float, float> m_x_scale;
    private IScaleContinuous<int, float> m_y_scale;

    ///////////////////////////
    /// AXES
    ///////////////////////////
    private Axis<float> m_x_axis;
    private Axis<int> m_y_axis;

    ///////////////////////////
    /// MATERIALS
    ///////////////////////////
    public Material AxisMaterial;
    public Material TickMaterial;
    public Material RectMaterial;

    ///////////////////////////
    /// DIMENSIONS
    ///////////////////////////
    private float m_width;  // in Unity's default unit
    private float m_height;  // in Unity's default unit
    private float m_bin_padding = 0.05f;

    private IGenerator2D<Bin[], Bin> m_rects;


    void Start()
    {
        // set the line chart dimensions
        m_width = 6;
        m_height = 6;

        // get and parse the data
        List<Record> data = Importers.FromResourcesCSV<Record>("Datasets/dataset_01",
            l => new Record(float.Parse(l))
        );

        if (data.Count == 0)
        {
            Debug.LogWarning("empty dataset - aborting visualization");
            return;
        }

        // compute the extent of relevant attribute(s)
        float min_val = 0;
        float max_val = 1000;
        // Utils.Extent(data, d => d.Value, out min_val, out max_val);

        // bin the data
        var bin_generator = new Bin<List<Record>, Record>()
            .Datum(data)
            .Domain(min_val, max_val)
            .Threshold(20)
            .Value(d => d.Value)
            .Generate(out Bin[] bins);

        // compute the extent of the bins
        Utils.Extent(bins, d => d.count, out float min_bin, out float max_bin);

        // create/update X scale
        m_x_scale = new ScaleLinearFloatFloat(min_val, max_val, 0, m_width);

        // create/update Y scale
        m_y_scale = new ScaleLinearIntFloat(0, (int)max_bin, 0, m_height);

        m_x_axis = new AxisBottom<float>(m_x_scale, tick_count: 2)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .Attach(gameObject);

        m_y_axis = new AxisLeft<int>(m_y_scale)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .Attach(gameObject);

        m_rects = new Primitive2D<Bin[], Bin>(
                data: bins,
                x_accessor: d => m_x_scale.F(d.x),
                y_accessor: d => m_y_scale.F(d.count) / 2.0f,
                width_accessor: d => m_x_scale.F(d.interval) - m_bin_padding,
                height_accessor: d => m_y_scale.F(d.count),
                shape: PrimitiveShape2D.RECT
            )
            .SetMaterial(RectMaterial)
            .Attach(gameObject);
    }
}