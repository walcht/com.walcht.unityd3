using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityD3
{
    public enum PrimitiveShape3D
    {
        CUBE,
        SPHERE
    }

    public class Primitive3D<T, D> where T : IEnumerable<D>
    {
        private GameObject m_container;
        private List<GameObject> m_primitives = new();
        private T m_data = default(T);
        private Func<D, float> m_x_accessor;
        private Func<D, float> m_y_accessor;
        private Func<D, float> m_z_accessor;
        private Func<D, float> m_width_accessor;
        private Func<D, float> m_height_accessor;
        private Func<D, float> m_depth_accessor;
        private Material m_material;

        public Primitive3D()
        {
            m_container = new GameObject("primitive");
        }

        public Primitive3D<T, D> Datum(T data)
        {
            m_data = data;
            foreach (GameObject go in m_primitives)
            {
                GameObject.Destroy(go);
            }
            m_primitives.Clear();
            return this;
        }

        public Primitive3D<T, D> Join(PrimitiveShape3D shape)
        {
            switch (shape)
            {
                case PrimitiveShape3D.CUBE:
                    {
                        int counter = 0;
                        Mesh mesh = Utils.GeneratePrimitiveMesh(shape);
                        foreach (D d in m_data)
                        {
                            GameObject go = new($"cube_{counter}", new Type[] {
                                typeof(MeshFilter),
                                typeof(MeshRenderer)
                            });
                            go.transform.SetParent(m_container.transform, worldPositionStays: false);
                            go.GetComponent<MeshFilter>().sharedMesh = mesh;
                            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
                            renderer.receiveShadows = false;
                            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                            m_primitives.Add(go);
                            ++counter;
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
            return this;
        }

        public Primitive3D<T, D> X(Func<D, float> accessor)
        {
            m_x_accessor = accessor;
            var go_iter = m_primitives.GetEnumerator();
            var data_iter = m_data.GetEnumerator();
            while (go_iter.MoveNext() && data_iter.MoveNext())
            {
                GameObject go = go_iter.Current;
                go.transform.localPosition = new Vector3(
                    m_x_accessor(data_iter.Current),
                    go.transform.localPosition.y,
                    go.transform.localPosition.z
                );
            }
            return this;
        }

        public Primitive3D<T, D> Y(Func<D, float> accessor)
        {
            m_y_accessor = accessor;
            var go_iter = m_primitives.GetEnumerator();
            var data_iter = m_data.GetEnumerator();
            while (go_iter.MoveNext() && data_iter.MoveNext())
            {
                GameObject go = go_iter.Current;
                go.transform.localPosition = new Vector3(
                    go.transform.localPosition.x,
                    m_y_accessor(data_iter.Current),
                    go.transform.localPosition.z
                );
            }
            return this;
        }

        public Primitive3D<T, D> Z(Func<D, float> accessor)
        {
            m_z_accessor = accessor;
            var go_iter = m_primitives.GetEnumerator();
            var data_iter = m_data.GetEnumerator();
            while (go_iter.MoveNext() && data_iter.MoveNext())
            {
                GameObject go = go_iter.Current;
                go.transform.localPosition = new Vector3(
                    go.transform.localPosition.x,
                    go.transform.localPosition.y,
                    m_z_accessor(data_iter.Current)
                );
            }
            return this;
        }

        public Primitive3D<T, D> Width(Func<D, float> accessor)
        {
            m_width_accessor = accessor;
            var go_iter = m_primitives.GetEnumerator();
            var data_iter = m_data.GetEnumerator();
            while (go_iter.MoveNext() && data_iter.MoveNext())
            {
                GameObject go = go_iter.Current;
                go.transform.localScale = new Vector3(
                    m_width_accessor(data_iter.Current),
                    go.transform.localScale.y,
                    go.transform.localScale.z
                );
            }
            return this;
        }

        public Primitive3D<T, D> Height(Func<D, float> accessor)
        {
            m_height_accessor = accessor;
            var go_iter = m_primitives.GetEnumerator();
            var data_iter = m_data.GetEnumerator();
            while (go_iter.MoveNext() && data_iter.MoveNext())
            {
                GameObject go = go_iter.Current;
                go.transform.localScale = new Vector3(
                    go.transform.localScale.x,
                    m_height_accessor(data_iter.Current),
                    go.transform.localScale.z
                );
            }
            return this;
        }

        public Primitive3D<T, D> Depth(Func<D, float> accessor)
        {
            m_depth_accessor = accessor;
            var go_iter = m_primitives.GetEnumerator();
            var data_iter = m_data.GetEnumerator();
            while (go_iter.MoveNext() && data_iter.MoveNext())
            {
                GameObject go = go_iter.Current;
                go.transform.localScale = new Vector3(
                    go.transform.localScale.x,
                    go.transform.localScale.y,
                    m_depth_accessor(data_iter.Current)
                );
            }
            return this;
        }

        public Primitive3D<T, D> Attach(GameObject game_object)
        {
            m_container.transform.SetParent(game_object.transform, worldPositionStays: false);
            m_container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            return this;
        }

        public Primitive3D<T, D> SetMaterial(Material mat)
        {
            m_material = new Material(mat);
            foreach (GameObject go in m_primitives)
            {
                go.GetComponent<MeshRenderer>().sharedMaterial = m_material;
            }
            return this;
        }

        public Primitive3D<T, D> SetColor(Color color)
        {
            m_material.color = color;
            return this;
        }
    }
}