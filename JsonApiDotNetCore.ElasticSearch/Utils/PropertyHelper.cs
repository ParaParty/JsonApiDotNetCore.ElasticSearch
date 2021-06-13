namespace JsonApiDotNetCore.ElasticSearch.Utils
{
    public class PropertyHelper
    {
        public static string GetPropName(string s)
        {
            var ret = s;
            if ('A' <= ret[0] && ret[0] <= 'Z')
            {
                ret = (char) (ret[0] - 'A' + 'a') + ret[1..];
            }
            return ret;
        }
    }
}
