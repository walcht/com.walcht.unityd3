using UnityEngine;
using UnityD3;
using System.Collections.Generic;

public class BasicHistogram3D : MonoBehaviour
{

    ///////////////////////////
    /// SCALES
    ///////////////////////////
    private IScaleContinuous<float, float> m_x_scale;
    private IScaleContinuous<int, float> m_y_scale;
    private IScaleContinuous<float, float> m_z_scale;

    ///////////////////////////
    /// AXES
    ///////////////////////////
    private Axis<float> m_x_axis;
    private Axis<int> m_y_axis;
    private Axis<float> m_z_axis;

    ///////////////////////////
    /// MATERIALS
    ///////////////////////////
    public Material AxisMaterial;
    public Material TickMaterial;
    public Material CubeMaterial;

    ///////////////////////////
    /// DIMENSIONS
    ///////////////////////////
    private float m_width;  // in Unity's default unit
    private float m_height;  // in Unity's default unit
    private float m_depth;  // in Unity's default unit
    private float m_bin_padding = 0.4f;
    private float m_min_height = 0.005f;  // so that cubes with height = 0 are correctly rendered


    private struct Record
    {
        public Record(float v0, float v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }
        public float v0;
        public float v1;
    }

    void Start()
    {
        // set the line chart dimensions
        m_width = 6;
        m_height = 6;
        m_depth = 6;

        // get and parse the data
        List<Record> data = Importers.FromResourcesCSV<Record>("dataset_02",
            l =>
            {
                string[] _split = l.Split();
                return new Record(float.Parse(_split[0]), float.Parse(_split[1]));
            }
        );

        if (data.Count == 0)
        {
            Debug.LogWarning("empty dataset - aborting visualization");
            return;
        }

        // compute the extent of relevant attribute(s)
        Utils.Extent(data, d => d.v0, out float min_val_x, out float max_val_x);
        Utils.Extent(data, d => d.v1, out float min_val_y, out float max_val_y);

        // bin the data
        var bin_generator = new Bin2D<List<Record>, Record>()
            .Datum(data)
            .DomainX(min_val_x, max_val_x)
            .DomainY(min_val_y, max_val_y)
            .NbrBinsX(10)
            .NbrBinsY(10)
            .X(d => d.v0)
            .Y(d => d.v1)
            .Generate(out Bin2D[] bins);

        // compute the extent of the bins
        Utils.Extent(bins, d => d.count, out int min_bin, out int max_bin);

        // create/update X scale
        m_x_scale = new ScaleLinearFloatFloat(0, max_val_x, 0, m_width);

        // create/update Y scale
        m_y_scale = new ScaleLinearIntFloat(0, max_bin, 0, m_height);

        // create/update Z scale
        m_z_scale = new ScaleLinearFloatFloat(0, max_val_y, 0, m_depth);

        // create/update X axis
        m_x_axis = new AxisBottom<float>(m_x_scale)
            .SetTickCount(4)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .SetTickLookAtCamera(true)
            .Attach(gameObject);

        // create/update Y axis
        m_y_axis = new AxisLeft<int>(m_y_scale)
            .SetTickCount(4)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .SetTickLookAtCamera(true)
            .Attach(gameObject);

        // create/update Z axis
        m_z_axis = new AxisBottom<float>(m_z_scale)
            .SetTickCount(4)
            .SetAxisMaterial(AxisMaterial)
            .SetTickMaterial(TickMaterial)
            .SetTickTextColor(Color.black)
            .SetTickLookAtCamera(true)
            .RotateAroundY(-90.0f)
            .Attach(gameObject);

        var rects = new Primitive3D<Bin2D[], Bin2D>()
            .Datum(bins)
            .Join(PrimitiveShape3D.CUBE)
            .X(d => m_x_scale.F(d.x))
            .Y(d => m_y_scale.F(d.count) / 2.0f)
            .Z(d => m_z_scale.F(d.y))
            .Width(d => m_x_scale.F(d.interval_x) - m_bin_padding)
            .Depth(d => m_z_scale.F(d.interval_y) - m_bin_padding)
            .Height(d => Mathf.Max(m_y_scale.F(d.count), m_min_height))
            .SetMaterial(CubeMaterial)
            .Attach(gameObject);
    }
}