using System;

namespace UnityD3
{
    /// <summary>
    ///     Base abstract class for continous scales. A continous scale is simply a continuous function that maps an
    ///     input range to an output range.
    /// </summary>
    /// <typeparam name="InputType">Type of the input domain</typeparam>
    /// <typeparam name="OutputType">Type of the output range</typeparam>
    public abstract class IScaleContinuous<InputType, OutputType> where InputType : unmanaged, IFormattable
    {
        protected InputType m_x0;
        protected InputType m_x1;
        protected OutputType m_y0;
        protected OutputType m_y1;
        protected string m_format = null;
        protected IFormatProvider m_format_provider = null;

        public delegate void DomainChangedEventHandler(InputType x0, InputType x1);
        public delegate void RangeChangedEventHandler(OutputType y0, OutputType y1);
        public event DomainChangedEventHandler DomainChanged;
        public event RangeChangedEventHandler RangeChanged;

        /// <summary>
        ///     Sets the scale's domain (i.e., input range) to [x0, x1]
        /// </summary>
        /// <param name="x0">domain's left extremety</param>
        /// <param name="x1">domain's right extremety</param>
        /// <returns>the scale on which this method was called</returns>
        public virtual IScaleContinuous<InputType, OutputType> Domain(InputType x0, InputType x1)
        {
            m_x0 = x0;
            m_x1 = x1;
            DomainChanged?.Invoke(m_x0, m_x1);
            return this;
        }

        /// <summary>
        ///     Sets the scale's range (i.e., output range) to [y0, y1]
        /// </summary>
        /// <param name="y0">range's left extremety</param>
        /// <param name="y1">range's right extremety</param>
        /// <returns>the scale on which this method was called</returns>
        public virtual IScaleContinuous<InputType, OutputType> Range(OutputType y0, OutputType y1)
        {
            m_y0 = y0;
            m_y1 = y1;
            RangeChanged?.Invoke(m_y0, m_y1);
            return this;
        }

        /// <summary>
        ///     Gets the scale's domain (i.e., input range [x0, x1])
        /// </summary>
        /// <param name="x0">domain's left extremety</param>
        /// <param name="x1">domain's right extremety</param>
        /// <returns>the scale on which this method was called</returns>
        public IScaleContinuous<InputType, OutputType> Domain(out InputType x0, out InputType x1)
        {
            x0 = m_x0;
            x1 = m_x1;
            return this;
        }


        /// <summary>
        ///     Gets the scale's range (i.e., output range [y0, y1])
        /// </summary>
        /// <param name="y0">output range</param>
        /// <param name="y1"></param>
        /// <returns>the scale on which this method was called</returns>
        public virtual void Range(out OutputType y0, out OutputType y1)
        {
            y0 = m_y0;
            y1 = m_y1;
        }

        /// <summary>
        ///     Sets the string format to use for generating domain value string representations. Affects TickText().
        /// </summary>
        /// <param name="format">string format applied to domain input type</param>
        /// <param name="format_provider">string format provider applied to domain input type</param>
        /// <returns>the scale on which this method was called</returns>
        public IScaleContinuous<InputType, OutputType> SetTickFormat(string format, IFormatProvider format_provider = null)
        {
            m_format = format;
            m_format_provider = format_provider;
            return this;
        }

        /// <summary>
        ///     Given a value x from the domain, returns the corresponding value from the range
        /// </summary>
        /// <param name="x">domain value to be mapped to its corresponding range value</param>
        /// <returns>range value</returns>
        public abstract OutputType F(InputType x);

        /// <summary>
        ///     Given a value y from the range, returns the corresponding value from the domain. This is simply the
        ///     inverse of F. Inversion is useful for interaction, say to determine the data value corresponding to the
        ///     position of the mouse.
        /// </summary>
        /// <param name="y">range value to be reverse mapped to its corresponding domain value</param>
        /// <returns>domain value</returns>
        public virtual InputType I(OutputType y) { throw new NotFiniteNumberException(); }

        /// <summary>
        ///     Generates approximatly count representative values from the scale's domain. This is useful for
        ///     positioning an axis' ticks. For integer input types, the number of generated ticks may be less than
        ///     count.
        /// </summary>
        /// <param name="count">inclusive upper bound of the number of representative values to generate from the domain</param>
        /// <returns>array of representative values with length <= count</returns>
        public abstract InputType[] Ticks(int count);

        /// <summary>
        ///     Generates string representation of a given domain value. Useful for generating texts for ticks.
        /// </summary>
        /// <param name="x">domain value</param>
        /// <returns>formatted string representation of a given domain value</returns>
        public virtual string TickText(InputType x) => x.ToString(m_format, m_format_provider);
    }
}