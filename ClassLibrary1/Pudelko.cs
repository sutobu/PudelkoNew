using System.Globalization;

namespace PudelkoLibrary
{
    public enum UnitOfMeasure
    {
        milimeter,
        centimeter,
        meter
    }

    public class Pudelko
    {
        private const double DefaultDimension = 0.1; //10cm
        private const double MaxDimension = 10.0; //10m

        private readonly double a;
        private readonly double b;
        private readonly double c;

        public double A => Math.Round(a, 3);
        public double B => Math.Round(b, 3);
        public double C => Math.Round(c, 3);
        public UnitOfMeasure Unit { get; }

        public Pudelko(double? a = null, double? b = null, double? c = null, UnitOfMeasure unit = UnitOfMeasure.meter)
        {
            this.a = Math.Round(ConvertToMeters(a ?? DefaultDimension, unit), 3);
            this.b = Math.Round(ConvertToMeters(b ?? DefaultDimension, unit), 3);
            this.c = Math.Round(ConvertToMeters(c ?? DefaultDimension, unit), 3);

            ValidateDimensions(this.a, this.b, this.c);
            Unit = unit;
        }

        private double ConvertToMeters(double value, UnitOfMeasure unit)
        {
            return unit switch
            {
                UnitOfMeasure.milimeter => value / 1000,
                UnitOfMeasure.centimeter => value / 100,
                UnitOfMeasure.meter => value,
                _ => throw new ArgumentOutOfRangeException(nameof(unit), "Invalid unit of measure")
            };
        }
        private void ValidateDimensions(params double[] dimensions)
        {
            foreach (var dimension in dimensions)
            {
                if (dimension <= 0 || dimension > MaxDimension)
                {
                    throw new ArgumentOutOfRangeException(nameof(dimensions), "Dimensions must be positive and less than or equal to 10 meters");
                }
            }
        }
        public override string ToString()
        {
            return ToString("m", null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) format = "m";

            return format switch
            {
                "m" => $"{A:F3} m × {B:F3} m × {C:F3} m",
                "cm" => $"{(A * 100):F1} cm × {(B * 100):F1} cm × {(C * 100):F1} cm",
                "mm" => $"{(A * 1000):F0} mm × {(B * 1000):F0} mm × {(C * 1000):F0} mm",
                _ => throw new FormatException($"The '{format}' format string is not supported.")
            };
        }
        public double Objetosc => Math.Round(a * b * c, 9);
        public double Pole => Math.Round(2 * (a * b + b * c + a * c), 6);

        public bool Equals(Pudelko other)
        {
            if (other == null) return false;

            var dimensions = new[] { A, B, C };
            var otherDimensions = new[] { other.A, other.B, other.C };

            Array.Sort(dimensions);
            Array.Sort(otherDimensions);

            return dimensions.SequenceEqual(otherDimensions);
        }

        public override bool Equals(object obj)
        {
            if (obj is Pudelko other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var dimensions = new[] { A, B, C };
            Array.Sort(dimensions);
            return HashCode.Combine(dimensions[0], dimensions[1], dimensions[2]);
        }

        public static bool operator ==(Pudelko left, Pudelko right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Pudelko left, Pudelko right)
        {
            return !(left == right);
        }

        public static Pudelko operator +(Pudelko p1, Pudelko p2)
        {
            double newA = p1.A + p2.A;
            double newB = p1.B + p2.B;
            double newC = p1.C + p2.C;

            return new Pudelko(newA, newB, newC, UnitOfMeasure.meter);
        }

        public static explicit operator double[](Pudelko p)
        {
            return new[] { p.A, p.B, p.C };
        }

        public static implicit operator Pudelko((int a, int b, int c) dimensions)
        {
            return new Pudelko(dimensions.a / 1000.0, dimensions.b / 1000.0, dimensions.c / 1000.0, UnitOfMeasure.meter);
        }

        public double this[int index]
        {
            get
            {
                return index switch
                {
                    0 => A,
                    1 => B,
                    2 => C,
                    _ => throw new IndexOutOfRangeException("Invalid index")
                };
            }
        }

        public IEnumerator<double> GetEnumerator()
        {
            yield return A;
            yield return B;
            yield return C;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Pudelko Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty");

            var parts = input.Split('×', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                throw new FormatException("Input string is not in the correct format");

            double[] dimensions = new double[3];
            UnitOfMeasure unit = UnitOfMeasure.meter;

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                var valueUnit = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (valueUnit.Length != 2)
                    throw new FormatException("Input string is not in the correct format");

                if (!double.TryParse(valueUnit[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    throw new FormatException("Input string is not in the correct format");

                unit = valueUnit[1] switch
                {
                    "m" => UnitOfMeasure.meter,
                    "cm" => UnitOfMeasure.centimeter,
                    "mm" => UnitOfMeasure.milimeter,
                    _ => throw new FormatException("Input string is not in the correct format")
                };

                dimensions[i] = unit switch
                {
                    UnitOfMeasure.meter => value,
                    UnitOfMeasure.centimeter => value / 100,
                    UnitOfMeasure.milimeter => value / 1000,
                    _ => throw new FormatException("Input string is not in the correct format")
                };
            }

            return new Pudelko(dimensions[0], dimensions[1], dimensions[2], UnitOfMeasure.meter);
        }
    }
}
