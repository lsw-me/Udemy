using UnityEngine;
using System;
using System.IO;
using System.Data;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";  //�������ݵ�·�� �ļ���


    private bool encryptData = false;//����
    private string codeWord = "Asuka";



    public FileDataHandler(string _dataDirPath,string _datdaFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _datdaFileName;
        encryptData = _encryptData;

    }

    public void Save(GameData _data) //�������л��ͷ����л�
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(_data , true);    //תΪjason ��ʽ�ļ�

            if (encryptData)                            //��Ҫ���ܾͼ���
                dataToStore = EncryptDecrpy(dataToStore);

            using (FileStream stream  = new FileStream(fullPath,FileMode.Create))
            {
                using (StreamWriter writer =  new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.Log("error on trying to save data to file" + fullPath + "\n" + e);
        }
    }


    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadData = null;


        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";//�ȿ� Ȼ����ļ��ж�ȡ
                using (FileStream stream = new FileStream(fullPath ,FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if(encryptData)
                    dataToLoad = EncryptDecrpy(dataToLoad); 

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log("error on try to load data " + fullPath + "\n" + e);
            }
        }
        return loadData;
    }

    public void Delete()  //����ɾ��������ļ������ڵ���
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if(File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string EncryptDecrpy(string _data)
    {
        string modifiedData = "";

        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }

}
