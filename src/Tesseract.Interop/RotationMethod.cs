namespace Tesseract.Interop
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Represents the method used to rotate an image.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum RotationMethod
    {
	    /// <summary>
	    ///     Use area map rotation, if possible.
	    /// </summary>
	    AreaMap = 1,

	    /// <summary>
	    ///     Use shear rotation.
	    /// </summary>
	    Shear = 2,

	    /// <summary>
	    ///     Use sampling.
	    /// </summary>
	    Sampling = 3
    }
}