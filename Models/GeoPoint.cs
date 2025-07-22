namespace AgroFieldApi.Models
{
    /// <summary>
    /// Представляет географическую точку с широтой и долготой.
    /// </summary>
    public class GeoPoint
    {
        /// <summary>
        /// Широта (latitude).
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Долгота (longitude).
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// Конструктор точки.
        /// </summary>
        /// <param name="lat">Широта</param>
        /// <param name="lng">Долгота</param>
        public GeoPoint(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }

        /// <summary>
        /// Удобное строковое представление для отладки.
        /// </summary>
        public override string ToString() => $"({Lat}, {Lng})";
    }
}