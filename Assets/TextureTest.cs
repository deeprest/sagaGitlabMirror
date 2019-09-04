using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTest : MonoBehaviour
{
  public ComputeShader shader;
  ComputeBuffer computeBuffer;
  ComputeBuffer computeBuffer2;
  int[] initialBufferData;
  int kernel;

  const int numThreads = 8;
  public int width = 64;
  public int height = 64;
  Texture2D texture;
  Timer timer;

  int[] old;

  void Start()
  {
    old = new int[width * height];

    texture = new Texture2D( width, height, TextureFormat.RGBA32, false );
    texture.filterMode = FilterMode.Point;
    GetComponent<Renderer>().material.mainTexture = texture;

    timer = new Timer();
    timer.unscaledTime = false;


    initialBufferData = new int[width * height];
    for( int i = 0; i < width * height; i++ )
    {
      if( Random.Range( 0, 2 ) > 0 )
        initialBufferData[i] = 0xFF;
      else
        initialBufferData[i] = 0x0;
    }

    // temp
    var colorData = texture.GetRawTextureData<Color32>();
    for( int i = 0; i < width * height; i++ )
    {
      colorData[i] = new Color32( (byte)initialBufferData[i], 0, 0, 255 );
    }
    texture.Apply();


    kernel = shader.FindKernel( "CSMain" );

    computeBuffer = new ComputeBuffer( width * height, sizeof( int ) );
    shader.SetBuffer( kernel, "A", computeBuffer );
    computeBuffer2 = new ComputeBuffer( width * height, sizeof( int ) );
    shader.SetBuffer( kernel, "B", computeBuffer2 );

    shader.SetInt( "bufferSize", width * height );
    shader.SetInt( "width", width );
    shader.SetInt( "height", height );
    //computeShader.SetInts( "offset", offset );

    //Conway( 1 );
    TimerRefresh();
  }

  private void OnDestroy()
  {
    computeBuffer.Release();
    computeBuffer.Dispose();
    computeBuffer2.Release();
    computeBuffer2.Dispose();
  }

  void TimerRefresh()
  {
    Conway();
    timer.Start( 1, null, TimerRefresh );
  }

  public void Conway()
  {
    int[] offset = {
      -1,
      1,
      width - 1,
      width,
      width + 1,
      -width - 1,
      -width,
      -width + 1
    };


    shader.SetBuffer( kernel, "buffer", computeBuffer );
    shader.SetBuffer( kernel, "old", computeBuffer2 );
    shader.SetInt( "bufferSize", width * height );

    shader.SetInts( "offset", offset );


    var colorData = texture.GetRawTextureData<Color32>();


    Color32 white = new Color32( 255, 255, 255, 255 );
    Color32 black = new Color32( 0, 0, 0, 0 );

    //computeBuffer.SetData( colorData );
    //computeShader.Dispatch( kernel, width / 8, height / 8, 1 );
    //computeBuffer2.GetData( old );


    /*
    int[] offset = {
      -1,
      1,
      width - 1,
      width,
      width + 1,
      -width - 1,
      -width,
      -width + 1
    };
    colorData.CopyTo( old );
    for( int x = 0; x < width; x++ )
    {
      for( int y = 0; y < height; y++ )
      {
        if( x == 0 || y == 0 || x == width - 1 || y == height - 1 )
          continue;
        int index = x + width * y;
        int n = 0;
        for( int i = 0; i < 8; i++ )
        {
          if( old[x + y * width + offset[i]] > 0 )
            n++;
        }
        if( old[index] > 0 )
        {
          if( n < 2 || n > 3 )
            colorData[index] = black;
        }
        else
        {
          if( n == 3 )
            colorData[index] = white;
        }
      }
    }*/



    //var result = new int[width * height];
    //computeBuffer.GetData( result );
    //for( int i = 0; i < width*height; i++ )
    //{
    //  print( result[i].ToString() );
    //}

    // RGBA32 texture format data layout exactly matches Color32 struct
    //var colorData = texture.GetRawTextureData<Color32>();
    //for( int i = 0; i < width * height; i++ )
    //colorData[i] = new Color32( (byte)colorData[i], 0, 0, 1 );
    //texture.SetPixels32( colorData.ToArray() );
    texture.Apply();


   
  }
}
