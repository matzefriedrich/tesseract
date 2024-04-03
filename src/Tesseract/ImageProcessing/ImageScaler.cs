namespace Tesseract.ImageProcessing
{
    using System;
    using Interop.Abstractions;

    public class ImageScaler
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public ImageScaler(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Scales the current pix by the specified <paramref name="scaleX" /> and <paramref name="scaleY" /> factors returning
        ///     a new <see cref="Pix" /> of the same depth.
        /// </summary>
        /// <param name="source">The source image to scale.</param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <returns>The scaled image.</returns>
        /// <remarks>
        ///     <para>
        ///         This function scales 32 bpp RGB; 2, 4 or 8 bpp palette color; 2, 4, 8 or 16 bpp gray; and binary images.
        ///     </para>
        ///     <para>
        ///         When the input has palette color, the colormap is removed and the result is either 8 bpp gray or 32 bpp RGB,
        ///         depending on whether the colormap has color entries.  Images with 2, 4 or 16 bpp are converted to 8 bpp.
        ///     </para>
        ///     <para>
        ///         Because Scale() is meant to be a very simple interface to a number of scaling functions, including the use of
        ///         unsharp masking, the type of scaling and the sharpening parameters are chosen by default.  Grayscale and color
        ///         images are scaled using one of four methods, depending on the scale factors:
        ///         <list type="number">
        ///             <item>
        ///                 <description>
        ///                     antialiased subsampling (lowpass filtering followed by subsampling, implemented here by area
        ///                     mapping), for scale factors less than 0.2
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     antialiased subsampling with sharpening, for scale factors between 0.2 and 0.7.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     linear interpolation with sharpening, for scale factors between 0.7 and 1.4.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     linear interpolation without sharpening, for scale factors >= 1.4.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///         One could use subsampling for scale factors very close to 1.0, because it preserves sharp edges.  Linear
        ///         interpolation blurs edges because the dest pixels will typically straddle two src edge pixels.  Subsmpling
        ///         removes entire columns and rows, so the edge is not blurred.  However, there are two reasons for not doing
        ///         this. First, it moves edges, so that a straight line at a large angle to both horizontal and vertical will have
        ///         noticable kinks where horizontal and vertical rasters are removed.  Second, although it is very fast, you get
        ///         good results on sharp edges by applying a sharpening filter.
        ///     </para>
        ///     <para>
        ///         For images with sharp edges, sharpening substantially improves the image quality for scale factors between
        ///         about 0.2 and about 2.0. pixScale() uses a small amount of sharpening by default because it strengthens edge
        ///         pixels that are weak due to anti-aliasing. The default sharpening factors are:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description><![CDATA[for scaling factors < 0.7:   sharpfract = 0.2    sharpwidth = 1]]></description>
        ///             </item>
        ///             <item>
        ///                 <description>for scaling factors >= 0.7:  sharpfract = 0.4    sharpwidth = 2</description>
        ///             </item>
        ///         </list>
        ///         The cases where the sharpening halfwidth is 1 or 2 have special implementations and are about twice as fast as
        ///         the general case.
        ///     </para>
        ///     <para>
        ///         However, sharpening is computationally expensive, and one needs to consider the speed-quality tradeoff:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     For upscaling of RGB images, linear interpolation plus default sharpening is about 5 times slower
        ///                     than upscaling alone.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     For downscaling, area mapping plus default sharpening is about 10 times slower than downscaling
        ///                     alone.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///         When the scale factor is larger than 1.4, the cost of sharpening, which is proportional to image area, is very
        ///         large compared to the incremental quality improvement, so we cut off the default use of sharpening at 1.4.
        ///         Thus, for scale factors greater than 1.4, pixScale() only does linear interpolation.
        ///     </para>
        ///     <para>
        ///         In many situations you will get a satisfactory result by scaling without sharpening: call pixScaleGeneral()
        ///         with @sharpfract = 0.0. Alternatively, if you wish to sharpen but not use the default value, first call
        ///         pixScaleGeneral() with @sharpfract = 0.0, and then sharpen explicitly using pixUnsharpMasking().
        ///     </para>
        ///     <para>
        ///         Binary images are scaled to binary by sampling the closest pixel, without any low-pass filtering (averaging of
        ///         neighboring pixels). This will introduce aliasing for reductions.  Aliasing can be prevented by using
        ///         pixScaleToGray() instead.
        ///     </para>
        ///     <para>
        ///         Warning: implicit assumption about RGB component order for LI color scaling
        ///     </para>
        /// </remarks>
        public Pix ScaleImage(Pix source, float scaleX, float scaleY)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IntPtr result = this.leptonicaApi.pixScale(source.Handle, scaleX, scaleY);

            if (result == IntPtr.Zero) throw new InvalidOperationException("Failed to scale pix.");

            return new Pix(this.leptonicaApi, result);
        }
    }
}