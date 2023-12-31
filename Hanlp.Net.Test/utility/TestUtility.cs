/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-23 11:05 PM</create-date>
 *
 * <copyright file="TestUtility.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.utility;


/**
 * @author hankcs
 */
public class TestUtility
{
    static TestUtility()
    {
        EnsureFullData();
    }

    public static void EnsureFullData()
    {
        EnsureData("data/model/crf", "http://nlp.hankcs.com/download.php?file=data", ".", false);
    }

    /**
     * 保证 name 存在，不存在时自动下载解压
     *
     * @param name 路径
     * @param url  下载地址
     * @return name的绝对路径
     */
    public static String EnsureData(String name, String url)
    {
        return EnsureData(name, url, null, true);
    }

    /**
     * 保证 name 存在，不存在时自动下载解压
     *
     * @param name 路径
     * @param url  下载地址
     * @return name的绝对路径
     */
    public static String EnsureData(String name, String url, String parentPath, bool overwrite)
    {
        var target =Path.Combine(Environment.CurrentDirectory, name);
        if (File.Exists(target)) return target;
        try
        {
            var parentFile = parentPath == null ? (name) : (parentPath);
            if (!Directory.Exists(parentFile)) Directory.CreateDirectory(parentFile);
            String filePath = DownloadFile(url, parentFile);
            if (filePath.EndsWith(".zip"))
            {
                Unzip(filePath, parentFile, overwrite);
            }
            return target;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("数据下载失败，请尝试手动下载 %s 到 %s 。原因如下：\n", url, target);
            //e.printStackTrace();
            Environment.Exit(1);
            return null;
        }
    }

    /**
     * 保证 data/test/name 存在
     *
     * @param name
     * @param url
     * @return
     */
    public static String EnsureTestData(String name, String url)
    {
        return EnsureData(String.Format("data/test/{0}", name), url);
    }

    /**
     * Downloads a file from a URL
     *
     * @param fileURL  HTTP URL of the file to be downloaded
     * @param savePath path of the directory to save the file
     * @
     * @author www.codejava.net
     */
    public static String DownloadFile(String fileURL, String savePath)        
    {
        return "";
//        Console.Error.WriteLine("Downloading %s to %s\n", fileURL, savePath);
//        Uri url = new Uri(fileURL);
//        HttpURLConnection httpConn = (HttpURLConnection) url.OpenConnection();
//        int responseCode = httpConn.getResponseCode();

//        // always check HTTP response code first
//        if (responseCode == HttpURLConnection.HTTP_OK)
//        {
//            String fileName = "";
//            String disposition = httpConn.getHeaderField("Content-Disposition");
//            String contentType = httpConn.getContentType();
//            int contentLength = httpConn.getContentLength();

//            if (disposition != null)
//            {
//                // extracts file name from header field
//                int index = disposition.IndexOf("filename=");
//                if (index > 0)
//                {
//                    fileName = disposition[(index + 10)..
//                                                    ( disposition.Length - 1)];
//                }
//            }
//            else
//            {
//                // extracts file name from URL
//                fileName = (httpConn.getURL().getPath()).getName();
//            }

////            Console.WriteLine("Content-Type = " + contentType);
////            Console.WriteLine("Content-Disposition = " + disposition);
////            Console.WriteLine("Content-Length = " + contentLength);
////            Console.WriteLine("fileName = " + fileName);

//            // opens input stream from the HTTP connection
//            Stream inputStream = httpConn.getInputStream();
//            String saveFilePath = savePath;
//            if (Directory.Exists(savePath))
//                saveFilePath = savePath + Path.DirectorySeparatorChar + fileName;
//            String realPath;
//            if (File.Exists(saveFilePath))
//            {
//                Console.Error.WriteLine("Use cached %s instead.\n", fileName);
//                realPath = saveFilePath;
//            }
//            else
//            {
//                saveFilePath += ".downloading";

//                // opens an output stream to save into file
//                var outputStream = new FileStream(saveFilePath, FileMode.Create);

//                int bytesRead;
//                byte[] buffer = new byte[4096];
//                long start = DateTime.Now.Microsecond;
//                int progress_size = 0;
//                while ((bytesRead = inputStream.Read(buffer)) != -1)
//                {
//                    outputStream.Write(buffer, 0, bytesRead);
//                    long duration = (DateTime.Now.Microsecond - start) / 1000;
//                    duration = Math.Max(duration, 1);
//                    progress_size += bytesRead;
//                    int speed = (int) (progress_size / (1024 * duration));
//                    float ratio = progress_size / (float) contentLength;
//                    float percent = ratio * 100;
//                    int eta = (int) (duration / ratio * (1 - ratio));
//                    int minutes = eta / 60;
//                    int seconds = eta % 60;

//                    Console.Error.WriteLine("\r%.2f%%, %d MB, %d KB/s, ETA %d min %d s", percent, progress_size / (1024 * 1024), speed, minutes, seconds);
//                }
//                Console.Error.WriteLine();
//                outputStream.Close();
//                realPath = saveFilePath[0 .. (saveFilePath.Length - ".downloading".Length)];
//                new FileInfo(saveFilePath).MoveTo((realPath));
//                //if (!)
//                //    throw new IOException("Failed to move file");
//            }

//            inputStream.Close();
//            httpConn.disconnect();

//            return realPath;
//        }
//        else
//        {
//            httpConn.disconnect();
//            throw new IOException("No file to download. Server replied HTTP code: " + responseCode);
//        }
    }

    private static void Unzip(String zipFilePath, String destDir, bool overwrite)
    {
        //Console.Error.WriteLine("Unzipping to " + destDir);
        //var dir =  (destDir);
        //// create output directory if it doesn't exist
        //if (!Directory.Exists( dir))Directory.CreateDirectory(dir);
        //FileStream fis;
        ////buffer for read and write data to file
        //byte[] buffer = new byte[4096];
        //try
        //{
        //    fis = new FileStream(zipFilePath, FileMode.Open);
        //    ZipInputStream zis = new ZipInputStream(fis);
        //    ZipEntry ze = zis.getNextEntry();
        //    while (ze != null)
        //    {
        //        String fileName = ze.getName();
        //        var newFile = (destDir + Path.DirectorySeparatorChar + fileName);
        //        if (overwrite || !File.Exists(newFile))
        //        {
        //            if (ze.isDirectory())
        //            {
        //                //create directories for sub directories in zip
        //                //newFile.mkdirs();
        //                Directory.CreateDirectory(newFile);
        //            }
        //            else
        //            {
        //                Directory.CreateDirectory(Path.GetDirectoryName(newFile));
        //                var fos = new FileStream(newFile, FileMode.Create);
        //                int len;
        //                while ((len = zis.read(buffer)) > 0)
        //                {
        //                    fos.Write(buffer, 0, len);
        //                }
        //                fos.Close();
        //                //Close this ZipEntry
        //                zis.closeEntry();
        //            }
        //        }
        //        ze = zis.getNextEntry();
        //    }
        //    //Close last ZipEntry
        //    zis.closeEntry();
        //    zis.Close();
        //    fis.Close();
        //    File.Delete(zipFilePath);
        //}
        //catch (IOException e)
        //{
        //    //e.printStackTrace();
        //}
    }
}
