﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Drawing;
using CSScriptLibrary;

namespace NESLevelEditor2
{
    public static class PluginLoader
    {
        public static T loadPlugin<T>(string path)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly currentAssembly = Assembly.LoadFile(Path.Combine(appPath, path));
            foreach (Type type in currentAssembly.GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(T)))
                    return (T)Activator.CreateInstance(type);
            }
            return default(T);
        }
    }

    //--------------------------------------------------------------------------------------------------------------
    public interface IPlugin
    {
        void addSubeditorButton(LevelEdit formMain);
        void addToolButton(LevelEdit formMain);
        void loadFromConfig(object asm, object data); //asm is CSScriptLibrary.AsmHelper
        string getName();
    }

    public interface IVideoPluginNes
    {
        void updateColorsFromConfig();

        Image[] makeBigBlocks(int videoNo, int bigBlockNo, int blockNo, int palleteNo, MapViewType smallObjectsViewType = MapViewType.Tiles,
            MapViewType curViewType = MapViewType.Tiles, int heirarchyLevel = 0);
        Image[] makeBigBlocks(int videoNo, int bigBlockNo, BigBlock[] bigBlockData, int palleteNo, MapViewType smallObjectsViewType = MapViewType.Tiles,
            MapViewType curViewType = MapViewType.Tiles, int heirarchyLevel = 0);

        Bitmap makeImage(int index, byte[] videoChunk, byte[] pallete, int subPalIndex, bool withAlpha = false);
        Bitmap makeImageStrip(byte[] videoChunk, byte[] pallete, int subPalIndex, bool withAlpha = false);
        Bitmap makeImageRectangle(byte[] videoChunk, byte[] pallete, int subPalIndex, bool withAlpha = false);

        Bitmap[] makeObjects(ObjRec[] objects, Bitmap[][] objStrips, MapViewType drawType, int constantSubpal = -1);
        Bitmap[] makeObjects(int videoPageId, int tilesId, int palId, MapViewType drawType, int constantSubpal = -1);
        Bitmap makeObject(int index, ObjRec[] objects, Bitmap[][] objStrips, MapViewType drawType, int constantSubpal = -1);
        Bitmap makeObjectsStrip(int videoPageId, int tilesId, int palId, MapViewType drawType, int constantSubpal = -1);

        Bitmap makeScreen(int scrNo, int levelNo, int videoNo, int bigBlockNo, int blockNo, int palleteNo, bool withBorders = true);

        Color[] NesColors { get; set; }

        string getName();
    }

    public interface IVideoPluginSega
    {
        Image[] makeBigBlocks(byte[] mapping, byte[] tiles, byte[] palette, int count, MapViewType curViewType = MapViewType.Tiles);
        Color[] GetPalette(byte[] pal);
        Bitmap GetTileFromArray(byte[] Tiles, ref int Position, Color[] Palette, byte PalIndex);
        Bitmap GetTileFrom2ColorArray(byte[] Tiles, ref int Position);
        byte[] GetArrayFrom2ColorTile(Bitmap tile);
        byte[] GetArrayFrom2ColorBlock(Bitmap block);
        //Bitmap GetBlockFrom2ColorArray(byte[] Tiles, int Width);
        Bitmap GetTile(byte[] tiles, ushort Word, Color[] palette, byte palIndex, bool HF, bool VF);
        // Bitmap GetBlock(ushort[] mapping, byte[] tiles, Color[] palette, byte Index);

        string getName();
    }
}
