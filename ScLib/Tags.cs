namespace ScLib
{
    public class Tags
    {
        /// <summary>
        /// Converts a id (player, clan...) into a hashtag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Id2Tag(long id)
        {
            var hashtag = string.Empty;

            var highInt = id >> 32;
            if (highInt > 255) return hashtag;

            var lowInt = id & 0xFFFFFFFF;

            id = (lowInt << 8) + highInt;
            while (id != 0)
            {
                var index = id % 14;
                hashtag = "0289PYLQGRJCUV"[(int) index] + hashtag;

                id /= 14;
            }

            hashtag = "#" + hashtag;

            return hashtag;
        }

        /// <summary>
        /// Convert a tag to a high-low id
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static int[] Tag2HighLow(string tag)
        {
            var tagArray = tag.Replace("#", "").ToUpper().ToCharArray();

            long id = 0;

            foreach (var t in tagArray)
            {
                id *= 14;
                id += "0289PYLQGRJCUV".IndexOf(t);
            }

            var high = (int) id % 256;
            var low = (int) (id - high) >> 8;

            return new [] {high, low};
        }

        /// <summary>
        /// Convert a tag to a id
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static long Tag2Id(string tag)
        {
            var tagArray = tag.Replace("#", "").ToUpper().ToCharArray();

            long id = 0;

            foreach (var t in tagArray)
            {
                id *= 14;
                id += "0289PYLQGRJCUV".IndexOf(t);
            }

            var high = (int) id % 256;
            var low = (int) (id - high) >> 8;

            return ((long) high << 32) | (low & 0xFFFFFFFFL);
        }
    }
}