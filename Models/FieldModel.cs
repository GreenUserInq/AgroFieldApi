using System.Collections.Generic;

namespace AgroFieldApi.Models
{
    /// <summary>
    /// Одно сельскохозяйственное поле
    /// </summary>
    public class FieldModel
    {
        private object _locations;
        /// <summary>
        /// Уникальный эдентификатор поля
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Название поля
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Площадь поля в кв м
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// Географический центр поля
        /// </summary>
        public GeoPoint Center { get; private set; }
        /// <summary>
        /// Контур поля по списку координат
        /// </summary>
        public List<GeoPoint> Polygon {  get; private set; }
        /// <summary>
        /// Структура координат: центр и полигон возвращает анонимный объект
        /// Кэшируется при первом обращении
        /// </summary>
        public object Location
        {
            get
            {
                return _locations ??= new
                {
                    Center = Center == null ? null : new[] { Center.Lat, Center.Lng },
                    Polygon = Polygon?.Select(p => new[] { p.Lat, p.Lng }).ToList()
                };
            }
        }

        /// <summary>
        /// Конструктор модели поля.
        /// </summary>
        /// <param name="id">Уникальный идентификатор поля.</param>
        /// <param name="name">Название поля.</param>
        /// <param name="center">Центр поля.</param>
        /// <param name="polygon">Контур поля.</param>
        /// <param name="size">Площадь (по умолчанию 0.0).</param>
        public FieldModel(string id, string name, GeoPoint center, List<GeoPoint> polygon, double size = 0.0) 
        { 
            Id = id;
            Name = name;
            Size = size;
            Center = center;
            Polygon = polygon;
        }

        /// <summary>
        /// Обновляет центр и контур поля, сбрасывает кэш
        /// </summary>
        /// <param name="center"></param>
        /// <param name="polygon"></param>
        public void UpdateGeometry(GeoPoint center, List<GeoPoint> polygon)
        {
            Center = center;
            Polygon = polygon;
            _locations = null;
        }
        /// <summary>
        /// Обновляет центр, сбрасывает кэш
        /// </summary>
        /// <param name="center"></param>
        public void UpdateCenter(GeoPoint center)
        {
            Center = center;
            _locations = null;
        }
        /// <summary>
        /// Обновляет контур, сьрасывает кэш
        /// </summary>
        /// <param name="polygon"></param>
        public void UpdatePolygon(List<GeoPoint> polygon)
        {
            Polygon = polygon;
            _locations = null;
        }
    }
}
