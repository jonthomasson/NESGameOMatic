using NESLevelEditor2;
using System.Collections.Generic;
//css_include Settings_TinyToon-Utils.cs;

public class Data
{ 
  public OffsetRec getPalOffset()       { return new OffsetRec(0xB1F0, 16, 16);        }
  public OffsetRec getVideoOffset()     { return new OffsetRec(0x4D10 , 1   , 0xD00);  }
  public OffsetRec getVideoObjOffset()  { return new OffsetRec(0x4D10 , 1   , 0xD00);  }
  public OffsetRec getBigBlocksOffset() { return new OffsetRec(0x45D9 , 1   , 0x4000); }
  public OffsetRec getBlocksOffset()    { return new OffsetRec(0x1F1CB, 1   , 0x440);  }
  public OffsetRec getScreensOffset()   { return new OffsetRec(0x4399, 12   , 48);     }
  public int getBigBlocksCount()        { return 62; }
  public int getScreenWidth()           { return 8; }
  public int getScreenHeight()          { return 6; }
  public IList<LevelRec> getLevelRecs() { return levelRecsTT; }
  public GetObjectsFunc getObjectsFunc() { return TinyToonUtils.getObjectsTT; }
  public SetObjectsFunc setObjectsFunc() { return TinyToonUtils.setObjectsTT; }
  public string getObjTypesPicturesDir() { return "obj_sprites_TT"; }
  public GetLayoutFunc  getLayoutFunc()  { return TinyToonUtils.getLayoutLinearTT;   }
  
  public GetVideoPageAddrFunc getVideoPageAddrFunc() { return getTinyToonVideoAddress; }
  public GetVideoChunkFunc    getVideoChunkFunc()    { return getTinyToonVideoChunk;   }
  public SetVideoChunkFunc    setVideoChunkFunc()    { return null; }
  public GetBigBlocksFunc     getBigBlocksFunc()     { return TinyToonUtils.getBigBlocksTT;}
  public SetBigBlocksFunc     setBigBlocksFunc()     { return TinyToonUtils.setBigBlocksTT;}
  public GetBlocksFunc        getBlocksFunc()        { return TinyToonUtils.getBlocks;}
  public SetBlocksFunc        setBlocksFunc()        { return TinyToonUtils.setBlocks;}
  public GetPalFunc           getPalFunc()           { return getPalleteLevel_1_2;}
  public SetPalFunc           setPalFunc()           { return null;}
  
  public IList<LevelRec> levelRecsTT = new List<LevelRec>() 
  {
    new LevelRec(0x14361, 14, 12, 1, 0x0),
  };
  
  public int getTinyToonVideoAddress(int id)
  {
    return -1;
  }
  
  public byte[] getTinyToonVideoChunk(int videoPageId)
  {
    return Utils.readVideoBankFromFile("videoBack_TT_11.bin", videoPageId);
  }
  
  public byte[] getPalleteLevel_1_2(int palId)
  {
    //for test level 1-1
    var pallete = new byte[] { 
      0x31, 0x0f, 0x27, 0x2a, 0x31, 0x0f, 0x27, 0x2b,
      0x31, 0x0f, 0x37, 0x17, 0x31, 0x0f, 0x20, 0x38
    }; 
    return pallete;
  }
  
  public bool isBigBlockEditorEnabled() { return true;  }
  public bool isBlockEditorEnabled()    { return true;  }
  public bool isEnemyEditorEnabled()    { return true; }
  //--------------------------------------------------------------------------------------------
}