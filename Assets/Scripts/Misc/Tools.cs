using System;
using System.IO;
using System.Text;

/*!
  \brief This class contains some usefull functions
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
class Tools
{
  /*!
    \brief Clean a string by removing space tab and return characters at it begining and at it end
    \param str The string to clean.
   */
  static public string epurStr(string str)
  {
    char[] charToStrip = {' ', '\t', '\n'};
    string strippedLine = str.TrimStart(charToStrip);
    strippedLine = strippedLine.TrimEnd(charToStrip);
    return strippedLine;
  }

  /*!
    \brief Get content of a file and encode it in UTF-8
    \param path The path of the file
   */
  static public MemoryStream getEncodedFileContent(string path)
  {
    StreamReader fileStream = new StreamReader(@path);
    string text = fileStream.ReadToEnd();
    fileStream.Close();
    byte[] encodedString = Encoding.UTF8.GetBytes(text);
    MemoryStream ms = new MemoryStream(encodedString);
    ms.Flush();
    ms.Position = 0;

    return ms;
  }
}