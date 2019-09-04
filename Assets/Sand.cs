using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scrolling texture buffer using chunks
public class Sand : MonoBehaviour
{
  public struct Pixel
  {
    public int type;
    //public Vector2 pos;
    //public Vector2 vel;
    //public float temp;
  }

  const int bw = 256;
  const int bh = 256;
  const int bsize = bw * bh;
  int bufferIndex = 0;
  const int numBuffers = 2;
  Pixel[][] buffers = new Pixel[numBuffers][];
  Pixel[] buffer;
  Pixel[] previous;


  const int chunkSize = 32;
  // starting index for update. This increases/decreases by chunk size
  int bufferStart = 0;


  // RENDER
  Texture2D texture;
  Dictionary<int, Color> colorMap = new Dictionary<int, Color>();
  Timer timer;

  // INTERACTION
  public int drawType = 0;

  private void Awake()
  {
    for( int i = 0; i < numBuffers; i++ )
    {
      buffers[i] = new Pixel[bsize];
    }
    buffer = buffers[0];

    for( int i = 0; i < bsize; i++ )
    {
      buffer[i] = new Pixel();
      buffer[i].type = Random.Range( 0, 2 );
    }

    timer = new Timer();
    timer.unscaledTime = false;

    // RENDER
    texture = new Texture2D( bw, bh, TextureFormat.RGBA32, false );
    texture.filterMode = FilterMode.Point;
    GetComponent<Renderer>().material.mainTexture = texture;
    colorMap.Add( 0, Color.clear );
    colorMap.Add( 1, Color.red );
    colorMap.Add( 2, Color.gray );

    //TimerUpdateLoop();
  }

  //void TimerUpdateLoop()
  //{
  //  UpdateSim();
  //  timer.Start( Time.timeScale * 0.003f, null, TimerUpdateLoop );
  //}

  // todo get index based on starting index in circular buffer
  int getIndex( int index, int plusX, int plusY )
  {
    // bufferStart
    // modulo with bw
    return 0;
  }


  private void UpdateSim()
  {
    // logic pass
    previous = buffers[bufferIndex];
    bufferIndex = (bufferIndex + 1) % numBuffers - 1;
    buffer = buffers[bufferIndex];

    for( int i = bw + 1; i < bsize - bw - 1; i++ )
    {
      if( i % bw == 0 || i % bw == bw - 1 || i == 0 || i > bsize - bw - 1 )
        continue;

      if( previous[i].type == 1 )
      {
        if( previous[i - bw].type == 0 )
        {
          previous[i].type = 0;
          buffer[i - bw].type = 1;
        }
        else
        if( previous[i - bw - 1].type == 0 )
        {
          previous[i].type = 0;
          buffer[i - bw - 1].type = 1;
        }
        else
          if( previous[i - bw + 1].type == 0 )
        {
          previous[i].type = 0;
          buffer[i - bw + 1].type = 1;
        }
      }
      else if( previous[i].type == 2 )
      {
        if( previous[i - bw].type == 0 )
        {
          previous[i].type = 0;
          buffer[i - bw].type = 2;
        }
      }

    }


    // movement passes

    // 2D physics interactions


    // RENDER
    var colorData = texture.GetRawTextureData<Color32>();
    for( int i = 0; i < bsize; i++ )
      colorData[i] = colorMap[buffer[i].type];
    texture.Apply();
  }

  float accum = 0;

  private void Update()
  {
    accum += Time.deltaTime;
    if( accum > 0.01f )
    {
      UpdateSim();
      accum = 0;
    }

    if( Input.GetMouseButton( 0 ) )
    {
      RaycastHit hit;
      Ray mr = Camera.main.ScreenPointToRay( Input.mousePosition );
      if( Physics.Raycast( mr, out hit ) )
      {
        Vector2Int p = new Vector2Int( Mathf.FloorToInt( hit.textureCoord.x * bw ), Mathf.FloorToInt( hit.textureCoord.y * bw ) );
        buffer[p.x + p.y * bw].type = drawType;
      }
    }
  }

}
