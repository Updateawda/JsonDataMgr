using System.IO;
using System.Text;
using UnityEngine;

public enum E_Json_Type
{
    JsonUtility
}
/// <summary>
/// Json���ݹ�����
/// </summary>
public class JsonDataMgr
{
    //��Կ
    private static byte S_ENCRYPT_KEY = 111;
    //Ĭ�ϴ洢·��
    private static string S_DEFAULT_URL = Application.persistentDataPath + @"\Json\";
    //�Զ����׺
    private static string S_SUFFIX = ".tp";

    /// <summary>
    /// ��������Ϣ�洢ΪJson��Ϣ
    /// </summary>
    /// <param name="obj">����</param>
    /// <param name="fileName">�ļ���</param>
    /// <param name="type">json�洢����</param>
    /// <param name="isEncrypt">�Ƿ����</param>
    public void SaveObjectToJson(object obj, string fileName, E_Json_Type type, bool isEncrypt = false)
    {
        //�����һ��û���ļ��� ����
        if (!Directory.Exists(S_DEFAULT_URL))
            Directory.CreateDirectory(S_DEFAULT_URL);

        //����תjson�ַ��� 
        string str = type == E_Json_Type.JsonUtility ? JsonUtility.ToJson(obj) : JsonMapper.ToJson(obj);
        //������
        if (!isEncrypt)
        {
            File.WriteAllText(S_DEFAULT_URL + fileName + S_SUFFIX, str);
        }
        else//����
        {
            //�õ��ַ����ֽ�����
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            //�۰�������
            //�ٱ���һ��
            for (int i = 0; i < bytes.Length / 2; ++i)
            {
                bytes[i] ^= S_ENCRYPT_KEY;
                bytes[bytes.Length / 2 + i] ^= S_ENCRYPT_KEY;
            }
            File.WriteAllBytes(S_DEFAULT_URL + fileName + S_SUFFIX, bytes);
        }
    }

    /// <summary>
    /// ����json�ļ��� �õ�����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName">�ļ���</param>
    /// <param name="type">json����</param>
    /// <param name="isEncrypt">�Ƿ����</param>
    /// <returns></returns>
    public T LoadObjectFromJson<T>(string fileName, E_Json_Type type, bool isEncrypt = false) where T : class
    {
        //ƴ��·��
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
            //�۰�������
            //�ٱ���һ��
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