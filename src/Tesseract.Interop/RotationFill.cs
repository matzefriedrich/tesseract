namespace Tesseract.Interop
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     What colour pixels should be used for the outside?
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum RotationFill
    {
	    /// <summary>
	    ///     Bring in white pixels from the outside.
	    /// </summary>
	    White = 1,

	    /// <summary>
	    ///     Bring in black pixels from the outside.
	    /// </summary>
	    Black = 2
    }
}