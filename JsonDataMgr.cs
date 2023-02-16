using System.IO;
using System.Text;
using UnityEngine;

public enum E_Json_Type
{
    JsonUtility
}
/// <summary>
/// Json数据管理器
/// </summary>
public class JsonDataMgr
{
    //秘钥
    private static byte S_ENCRYPT_KEY = 111;
    //默认存储路径
    private static string S_DEFAULT_URL = Application.persistentDataPath + @"\Json\";
    //自定义后缀
    private static string S_SUFFIX = ".tp";

    /// <summary>
    /// 将对象信息存储为Json信息
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fileName">文件名</param>
    /// <param name="type">json存储类型</param>
    /// <param name="isEncrypt">是否加密</param>
    public void SaveObjectToJson(object obj, string fileName, E_Json_Type type, bool isEncrypt = false)
    {
        //如果第一次没有文件夹 创建
        if (!Directory.Exists(S_DEFAULT_URL))
            Directory.CreateDirectory(S_DEFAULT_URL);

        //对象转json字符串 
        string str = type == E_Json_Type.JsonUtility ? JsonUtility.ToJson(obj) : JsonMapper.ToJson(obj);
        //不加密
        if (!isEncrypt)
        {
            File.WriteAllText(S_DEFAULT_URL + fileName + S_SUFFIX, str);
        }
        else//加密
        {
            //得到字符串字节数组
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            //折半异或加密
            //少遍历一半
            for (int i = 0; i < bytes.Length / 2; ++i)
            {
                bytes[i] ^= S_ENCRYPT_KEY;
                bytes[bytes.Length / 2 + i] ^= S_ENCRYPT_KEY;
            }
            File.WriteAllBytes(S_DEFAULT_URL + fileName + S_SUFFIX, bytes);
        }
    }

    /// <summary>
    /// 根据json文件名 得到对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="type">json类型</param>
    /// <param name="isEncrypt">是否加密</param>
    /// <returns></returns>
    public T LoadObjectFromJson<T>(string fileName, E_Json_Type type, bool isEncrypt = false) where T : class
    {
        //拼接路径
        string path = S_DEFAULT_URL + fileName + S_SUFFIX;

        if (!File.Exists(path))
            return null;

        if (!isEncrypt)
        {
            return type == E_Json_Type.JsonUtility ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : JsonMapper.ToObject<T>(File.ReadAllText(path));
        }
        else
        {
            byte[] bytes = File.ReadAllBytes(path);
            //折半异或解密
            //少遍历一半
            for (int i = 0; i < bytes.Length / 2; ++i)
            {
                bytes[i] ^= S_ENCRYPT_KEY;
                bytes[bytes.Length / 2 + i] ^= S_ENCRYPT_KEY;
            }
            string str = Encoding.UTF8.GetString(bytes);
            return type == E_Json_Type.JsonUtility ? JsonUtility.FromJson<T>(str) : JsonMapper.ToObject<T>(str);
        }
    }
}