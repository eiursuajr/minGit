using System;
using System.Collections.Generic;
using System.Text;

namespace Ektron.Cms.Workarea.Reports
{
	/// <summary>
	/// Manages list of data for charts, including Google charts api
	/// </summary>
	public class ChartData
	{
		static public ChartData CreateChartData(List<int> data)
		{
			List<double> values = new List<double>(data.Count);
			foreach (int value in data)
			{
				values.Add((double)value);
			}
			return new ChartCountData(values);
		}

		static public ChartData CreateChartData(List<float> data)
		{
			List<double> values = new List<double>(data.Count);
			foreach (float value in data)
			{
				values.Add((double)value);
			}
			return new ChartPercentData(values);
		}

		static public ChartData CreateChartData(List<TimeSpan> data)
		{
			List<double> values = new List<double>(data.Count);
			foreach (TimeSpan value in data)
			{
				values.Add(value.TotalMinutes);
			}
			return new ChartTimeData(values);
		}

		static public ChartData CreateChartData(List<double> data)
		{
			return new ChartData(data);
		}

		protected List<double> _values = null;

		public ChartData(List<double> data)
		{
			_values = data;
		}

		public virtual Type DataType
		{
			get
			{
				return typeof(double);
			}
		}

		public virtual void Pad(int count)
		{
			for (int i = 0; i < count; i++)
			{
				_values.Add(-1.0);
			}
		}

		public int Count
		{
			get
			{
				return _values.Count;
			}
		}

		public double Max()
		{
			double max = 0.0;
			for (int i = 0; i < _values.Count; i++)
			{
				if (_values[i] > max)
				{
					max = _values[i];
				}
			}
			return max;
		}

		public virtual double GetChartScale(double value)
		{
			if (value <= 1.0) return 1.0;
			double magnitude = Math.Pow(10.0, Math.Floor(Math.Log10(value)));
			return (Math.Ceiling(value / magnitude) * magnitude);
		}

		public virtual string FormatChartLabel(double value)
		{
			return value.ToString("#,##0.00");
		}

		public virtual string EncodeGoogleChartData(double scale)
		{
			StringBuilder sb = new StringBuilder();
			if (1 == _values.Count)
			{
				// Google won't plot just one point
				_values.Add(_values[0]);
			}
			foreach (double value in _values)
			{
				sb.Append(EncodeGoogleChartValue(value, scale));
			}
			return sb.ToString();
		}

		// http://code.google.com/apis/chart/formats.html#simple
		// (essentially base62 encoding)
		private char[] _arySimpleEncoding = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
		private const int _SimpleEncodingBase = 62;

		private char EncodeGoogleChartValue(double value, double scale)
		{
            if (0.0 == scale) return _arySimpleEncoding[0];
            else if (value < 0.0 || scale <= 0.0) return '_';
			int scaledValue = (int)Math.Floor((value / scale) * _SimpleEncodingBase);
			if (scaledValue == _SimpleEncodingBase) scaledValue -= 1;
			return _arySimpleEncoding[scaledValue];
		}
	}

	public class ChartCountData : ChartData
	{
		public ChartCountData(List<double> data)
			: base(data)
		{
		}

		public override Type DataType
		{
			get
			{
				return typeof(int);
			}
		}

		public override double GetChartScale(double value)
		{
			if (value <= 10.0) return 10.0;
			return base.GetChartScale(value);
		}

		public override string FormatChartLabel(double value)
		{
			return ((int)value).ToString("#,##0");
		}
	}

	public class ChartPercentData : ChartData
	{
		public ChartPercentData(List<double> data)
			: base(data)
		{
		}

		public override Type DataType
		{
			get
			{
				return typeof(float);
			}
		}

		public override string FormatChartLabel(double value)
		{
			return value.ToString("0.00%"); // don't use {0:P} because it separates the number and the "%" with a space
		}
	}

	public class ChartTimeData : ChartData
	{
		public ChartTimeData(List<double> data)
			: base(data)
		{
		}

		public override Type DataType
		{
			get
			{
				return typeof(TimeSpan);
			}
		}

		public override string FormatChartLabel(double value)
		{
			return TimeSpan.FromMinutes(value).ToString();
		}
	}
}