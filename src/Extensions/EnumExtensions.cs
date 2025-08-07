using System.ComponentModel;

/// <summary>
/// Cung cấp các phương thức mở rộng (extension methods) cho các dữ liệu kiểu Enum.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Lấy chuỗi mô tả (Description) của một giá trị Enum.
    /// </summary>
    /// <remarks>
    /// Phương thức này sử dụng Reflection để đọc thuộc tính DescriptionAttribute
    /// được gán cho giá trị Enum. Nếu không tìm thấy thuộc tính, nó sẽ
    /// trả về tên của giá trị Enum dưới dạng chuỗi.
    /// </remarks>
    /// <param name="value">Giá trị Enum cần lấy mô tả.</param>
    /// <returns>Chuỗi mô tả của giá trị Enum hoặc tên của giá trị đó nếu không có mô tả.</returns>
    public static string GetDescription(this Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());

        if (fi != null)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
                return attributes[0].Description;
        }

        return value.ToString(); // fallback nếu không có Description
    }
}