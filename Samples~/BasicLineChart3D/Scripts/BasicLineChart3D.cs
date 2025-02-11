using UnityEngine;
using UnityD3;
using System.Collections.Generic;

public class BasicLineChart3D : MonoBehaviour
{

    ///////////////////////////
    /// SCALES
    ///////////////////////////
    private IScaleContinuous<float, float> m_x_scale;
    private IScaleContinuous<float, float> m_y_scale;
    private IScaleContinuous<float, float> m_z_scale;

    ///////////////////////////
    /// AXES
    ///////////////////////////
    private Axis<float> m_x_axis;
    private Axis<float> m_y_axis;
    private Axis<float> m_z_axis;

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
    private float m_depth;  // in Unity's default unit

    private IGenerator3D<List<Record>, Record> m_line;

    private struct Record
    {
        public Record(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
    }

    void Start()
    {
        // set the line chart dimensions
        m_width = 6;
        m_height = 6;
        m_depth = 6;

        // get and parse the data
        List<Record> data = Importers.FromResourcesCSV<Record>("dataset_03",
            l =>
            {
                string[] _split = l.Split(",");
                return new Record(float.Parse(_split[0]), float.Parse(_split[1]), float.Parse(_split[2]));
            }
        );

        if (data.Count == 0)
        {
            Debug.LogWarning("empty dataset - aborting visualization");
            return;
        }

        // compute the extent of relevant attribute(s)
        Utils.Extent(data, d => d.x, out float min_val_x, out float max_val_x);
        Utils.Extent(data, d => d.y, out float min_val_y, out float max_val_y);
        Utils.Extent(data, d => d.z, out float min_val_z, out float max_val_z);

        // create/update X scale
        m_x_scale = new ScaleLinearFloatFloat(min_val_x, max_val_x, 0, m_width);

        // create/update Y scale
        m_y_scale = new ScaleLinearFloatFloat(min_val_z, max_val_z, 0, m_height);

        // create/update Z scale
        m_z_scale = new ScaleLinearFloatFloat(min_val_y, max_val_y, 0, m_depth);

        // create/update X axis
        m_x_axis = new AxisBottom<float>(m_x_scale, tick_count: 4)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .SetTickLookAtCamera(true)
            .Attach(gameObject);

        // create/update Y axis
        m_y_axis = new AxisLeft<float>(m_y_scale, tick_count: 4)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .SetTickLookAtCamera(true)
            .Attach(gameObject);

        // create/update Z axis
        m_z_axis = new AxisBottom<float>(m_z_scale, tick_count: 4)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .SetTickLookAtCamera(true)
            .RotateAroundY(-90.0f)
            .Attach(gameObject);


        // create/update the line
        m_line = new Line3D<List<Record>, Record>(
                data: data,
                x_accessor: d => m_x_scale.F(d.x),
                y_accessor: d => m_y_scale.F(d.y),
                z_accessor: d => m_z_scale.F(d.z))
            .SetMaterial(LineMaterial)
            .SetStrokeWidth(0.05f)
            .Attach(gameObject);
    }
}