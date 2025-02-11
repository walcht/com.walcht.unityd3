using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityD3;

public class BasicLineChart2D : MonoBehaviour
{

    ///////////////////////////
    /// SCALES
    ///////////////////////////
    private IScaleContinuous<DateTime, float> m_x_scale;
    private IScaleContinuous<float, float> m_y_scale;

    ///////////////////////////
    /// AXES
    ///////////////////////////
    private Axis<DateTime> m_x_axis;
    private Axis<float> m_y_axis;

    ///////////////////////////
    /// MATERIALS
    ///////////////////////////
    public Material AxisMaterial;
    public Material TickMaterial;
    public Material LineMaterial;


    ///////////////////////////
    /// DIMENSIONS
    ///////////////////////////
    private float m_width;  // in Unity's default unit
    private float m_height;  // in Unity's default unit

    private IGenerator2D<List<Record>, Record> m_line;

    private struct Record
    {
        public Record(DateTime date, float value)
        {
            Date = date;
            Value = value;
        }

        public DateTime Date;
        public float Value;
    }

    void Start()
    {
        // set the line chart dimensions
        m_width = 6;
        m_height = 6;

        // get and parse the data
        List<Record> data = Importers.FromResourcesCSV<Record>("dataset_00",
            l =>
            {
                string[] _split = l.Split(",");
                return new Record(
                DateTime.ParseExact(_split[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                float.Parse(_split[1])
            );
            }
        );

        if (data.Count == 0)
        {
            Debug.LogWarning("empty dataset - aborting visualization");
            return;
        }

        // compute the extent of relevant attributes
        Utils.Extent(data, d => d.Date, out DateTime min_date, out DateTime max_date);
        Utils.Extent(data, d => d.Value, out float min_val, out float max_val);

        // create/update X scale and Axis
        m_x_scale = new ScaleLinearTimeFloat(min_date, max_date, 0, m_width)
            .SetTickFormat("dd-MM-yy");

        // create/update Y scale and Axis
        m_y_scale = new ScaleLinearFloatFloat(min_val, max_val, 0, m_height);

        m_x_axis = new AxisBottom<DateTime>(m_x_scale, tick_count: 2)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .Attach(gameObject);

        m_y_axis = new AxisLeft<float>(m_y_scale, tick_count: 4)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .Attach(gameObject);

        // create/update the line
        m_line = new Line2D<List<Record>, Record>(
                data: data,
                x_accessor: d => m_x_scale.F(d.Date),
                y_accessor: d => m_y_scale.F(d.Value))
            .SetMaterial(LineMaterial)
            .SetStrokeWidth(0.04f)
            .SetColor(new Color(105 / 255.0f, 179 / 255.0f, 162 / 255.0f))
            .Attach(gameObject);
    }
}
