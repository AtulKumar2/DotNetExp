using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TagLib; // http://developer.novell.com/wiki/index.php/TagLib_Sharp

namespace DotNetExp
{
    class MediaFiles
    {
        public void Run()
        {
            ArrayList MediaFileList = new ArrayList();

            string[] GenreList = { "Other", "Others" };
            MediaFileList.AddRange(MediaFilesWithGivenAttribute(
                    ATTRIBUTES.GENRE, GenreList, @"G:\Music\HindiMovies"));

            //foreach (string MediaFile in MediaFileList)
            //    Console.WriteLine(MediaFile);

            Dictionary<string, string> AlbumList = AllAlbumsFromMediaFileList(MediaFileList);

            return;
        }

        private enum ATTRIBUTES
        {
            GENRE = 0,
            ALBUM,
            NAME
        }

        Dictionary<string, string> AllAlbumsFromMediaFileList(ArrayList BasicMediaFileInfoList)
        {
            if (null == BasicMediaFileInfoList) return null;
            
            Dictionary<string, string> AlbumList = new Dictionary<string,string>();
            foreach (BasicMediaFileInfo file in BasicMediaFileInfoList)
            {
                string Album = file.AlbumName;
                if ((null == Album || 0 == Album.Length) && 
                    (null != file.DirectoryPath && 0 != file.DirectoryPath.Length))
                {
                    string[] Tokenized = file.DirectoryPath.Split('\\');
                    Album = Tokenized[Tokenized.Length - 1];
                }
                if (null == Album || AlbumList.ContainsKey(Album.ToLower())) continue;
                AlbumList.Add(Album.ToLower(), file.DirectoryPath);
            }
            return AlbumList;
        }

        struct BasicMediaFileInfo
        {
            public string FilePath;
            public string AlbumName;
            public string DirectoryPath;
        }

        ArrayList MediaFilesWithGivenAttribute(ATTRIBUTES Attrib, object AttribData, string RootPath)
        {
            DirectoryInfo RootDir = new DirectoryInfo(RootPath);
            FileInfo [] FileList = RootDir.GetFiles("*.*", SearchOption.AllDirectories);
            ArrayList ReturnList = new ArrayList();

            foreach (FileInfo file in FileList)
            {
                try 
                {
                    TagLib.File MediaFile = TagLib.File.Create(file.FullName);

                    switch (Attrib)
                    {
                        case ATTRIBUTES.GENRE:
                            if (null == MediaFile.Tag.Genres || null == AttribData 
                                || 0 == ((string [])AttribData).Length) break;

                            foreach (string Tag in MediaFile.Tag.Genres)
                            {
                                foreach(string Genre in (string [])AttribData)
                                if (0 == String.Compare(Genre, Tag, true))
                                {
                                    BasicMediaFileInfo BMFInfo = new BasicMediaFileInfo();
                                    BMFInfo.FilePath = file.FullName;
                                    BMFInfo.DirectoryPath = file.Directory.FullName;
                                    BMFInfo.AlbumName = MediaFile.Tag.Album;
                                    ReturnList.Add(BMFInfo);
                                    break;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }             
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return ReturnList;
        }
    }
}
