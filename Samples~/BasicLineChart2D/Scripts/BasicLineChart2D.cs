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
    [SerializeField, UnityEngine.Range(1.0f, 10.0f)] private float m_width = 9;  // in Unity's default unit

    [SerializeField, UnityEngine.Range(1.0f, 10.0f)] private float m_height = 6;  // in Unity's default unit

    [SerializeField, UnityEngine.Range(1, 20)] private int m_x_axis_tick_count = 5;
    [SerializeField, UnityEngine.Range(1, 20)] private int m_y_axis_tick_count = 7;

    [SerializeField] private float m_y_min_domain;
    [SerializeField] private float m_y_max_domain;

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
        Utils.Extent(data, d => d.Value, out m_y_min_domain, out m_y_max_domain);

        // create/update X scale and Axis
        m_x_scale = new ScaleLinearTimeFloat(min_date, max_date, 0, m_width)
            .SetTickFormat("dd-MM-yy");

        // create/update Y scale and Axis
        m_y_scale = new ScaleLinearFloatFloat(m_y_min_domain, m_y_max_domain, 0, m_height)
            .SetTickFormat("C2");

        m_x_axis = new AxisBottom<DateTime>(m_x_scale, tick_count: m_x_axis_tick_count)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .Update()
            .Attach(gameObject);

        m_y_axis = new AxisLeft<float>(m_y_scale, tick_count: m_y_axis_tick_count)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .Update()
            .Attach(gameObject);

        // create/update the line
        m_line = new Line2D<List<Record>, Record>(
                data: data,
                x_accessor: d => m_x_scale.F(d.Date),
                y_accessor: d => m_y_scale.F(d.Value),
                filter: d => d.Date < min_date || d.Date > max_date)
            .SetMaterial(LineMaterial)
            .SetStrokeWidth(0.04f)
            .SetColor(new Color(105 / 255.0f, 179 / 255.0f, 162 / 255.0f))
            .Update()
            .Attach(gameObject);
    }

    void Update()
    {
        m_x_scale.Range(out float _, out float _width);
        m_y_scale.Range(out float _, out float _height);
        if ((_width != m_width) && (_height != m_height))
        {
            m_x_scale.Range(0, m_width);
            m_y_scale.Range(0, m_height);
            m_line.ForceUpdate();
        }
        else if (_width != m_width)
        {
            m_x_scale.Range(0, m_width);
            m_line.ForceUpdate();
        }
        else if (_height != m_height)
        {
            m_y_scale.Range(0, m_height);
            m_line.ForceUpdate();
        }

        m_y_scale.Domain(out float _y_min_domain, out float _y_max_domain);
        if ((_y_min_domain != m_y_min_domain) || (_y_max_domain != m_y_max_domain))
        {
            m_y_scale.Domain(m_y_min_domain, m_y_max_domain);
            m_line.ForceUpdate();
        }

        // no need to perform checks as these are already performed in SetTickCount
        m_x_axis.SetTickCount(m_x_axis_tick_count);
        m_y_axis.SetTickCount(m_y_axis_tick_count);

        m_x_axis.Update();
        m_y_axis.Update();
        m_line.Update();
    }
}
