using System.Collections.Generic;
using System.Text;

namespace ScLib.SC
{
    public class ScInfoFile
    {
        public List<Export> Exports = new List<Export>();

        public ushort ShapeCount,
            ExportCount,
            MovieClipCount,
            TextureCount,
            TextCount,
            MatrixCount,
            ColorTransformCount;

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Shapes:          {ShapeCount}");
            builder.AppendLine($"Exports:         {ExportCount}");
            builder.AppendLine($"MovieClips:      {MovieClipCount}");
            builder.AppendLine($"Textures:        {TextureCount}");
            builder.AppendLine($"Texts:           {TextCount}");
            builder.AppendLine($"Matrixs:         {MatrixCount}");
            builder.AppendLine($"ColorTransforms: {ColorTransformCount}");

            return builder.ToString();
        }
    }
}