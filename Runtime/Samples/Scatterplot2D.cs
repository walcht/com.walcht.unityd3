using System.Collections.Generic;
using UnityEngine;
using UnityD3;

public class Scatterplot2D : MonoBehaviour
{
    private struct Record
    {
        public Record(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x;
        public int y;
    }

    ///////////////////////////
    /// SCALES
    ///////////////////////////
    private IScaleContinuous<int, float> m_x_scale;
    private IScaleContinuous<int, float> m_y_scale;

    ///////////////////////////
    /// AXES
    ///////////////////////////
    private Axis<int> m_x_axis;
    private Axis<int> m_y_axis;

    ///////////////////////////
    /// MATERIALS
    ///////////////////////////
    public Material AxisMaterial;
    public Material TickMaterial;
    public Material CircleMaterial;


    ///////////////////////////
    /// DIMENSIONS
    ///////////////////////////
    private float m_width;  // in Unity's default unit
    private float m_height;  // in Unity's default unit

    private IGenerator2D<List<Record>, Record> m_circles;

    void Start()
    {
        // set the line chart dimensions
        m_width = 6;
        m_height = 6;

        // get and parse the data
        List<Record> data = Importers.FromResourcesCSV<Record>("Datasets/dataset_04",
            l =>
            {
                string[] _split = l.Split(",");
                return new Record(int.Parse(_split[0]), int.Parse(_split[1]));
            }
        );

        if (data.Count == 0)
        {
            Debug.LogWarning("empty dataset - aborting visualization");
            return;
        }

        // compute the extent of relevant attributes
        Utils.Extent(data, d => d.x, out int min_val_x, out int max_val_x);
        Utils.Extent(data, d => d.y, out int min_val_y, out int max_val_y);

        // create/update X scale
        m_x_scale = new ScaleLinearIntFloat(0, max_val_x, 0, m_width);

        // create/update Y scale
        m_y_scale = new ScaleLinearIntFloat(0, max_val_y, 0, m_height)
            .SetTickFormat("C0");

        m_x_axis = new AxisBottom<int>(m_x_scale, tick_count: 5)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .Attach(gameObject);

        m_y_axis = new AxisLeft<int>(m_y_scale, tick_count: 5)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .Attach(gameObject);

        // create/update the primitives
        m_circles = new Primitive2D<List<Record>, Record>(
                data: data,
                x_accessor: d => m_x_scale.F(d.x),
                y_accessor: d => m_y_scale.F(d.y),
                width_accessor: d => 0.05f,
                height_accessor: d => 0.05f,
                shape: PrimitiveShape2D.CIRCLE
            )
            .SetMaterial(CircleMaterial)
            .Attach(gameObject);
    }
}
