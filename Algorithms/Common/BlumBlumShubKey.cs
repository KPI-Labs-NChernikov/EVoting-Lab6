namespace Algorithms.Common;
public struct BlumBlumShubKey
{
    public byte[] P { get; set; }
    public byte[] Q { get; set; }
    public byte[] N { get; set; }
    public byte[]? X0 { get; set; }

    public BlumBlumShubKey WithX0(byte[] x0)
    {
        X0 = x0;
        return this;
    }
}
