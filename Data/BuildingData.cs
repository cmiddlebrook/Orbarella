using System.Collections.Generic;

namespace Orbarella;

public class BuildingData
{
    public string Name { get; set; }
    public List<WindowPosition> Windows { get; set; }
}

public class WindowPosition
{
    public int X { get; set; }
    public int Y { get; set; }
}