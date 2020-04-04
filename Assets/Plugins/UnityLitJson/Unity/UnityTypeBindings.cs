#if !JSON_STANDALONE

using UnityEngine;
using System;
using System.Collections;

using LitJson.Extensions;

namespace LitJson {

	#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
	#endif
	public static class UnityTypeBindings {

		static bool registerd;

		static UnityTypeBindings(){
			Register();
		}

		public static void Register(){

			if(registerd) return;
			registerd = true;

			// If you seralize using WriteProperty()
			// LitJson will attempt to bind property
			// names to class members instead of using
			// an importer.

			// -- Type
			JsonMapper.RegisterExporter<Type>((v,w) => {
				w.Write(v.FullName);
			});
			
			JsonMapper.RegisterImporter<string,Type>((s) => {
				return Type.GetType(s);
			});

			// -- Vector2
			Action<Vector2,JsonWriter> writeVector2 = (v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("x",v.x);
				w.WriteProperty("y",v.y);
				w.WriteObjectEnd();
			};

			JsonMapper.RegisterExporter<Vector2>((v,w) => {
				writeVector2(v,w);
			});

			// -- Vector3
			Action<Vector3,JsonWriter> writeVector3 = (v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("x",v.x);
				w.WriteProperty("y",v.y);
				w.WriteProperty("z",v.z);
				w.WriteObjectEnd();
			};

			JsonMapper.RegisterExporter<Vector3>((v,w) => {
				writeVector3(v,w);
			});

			// -- Vector4
			JsonMapper.RegisterExporter<Vector4>((v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("x",v.x);
				w.WriteProperty("y",v.y);
				w.WriteProperty("z",v.z);
				w.WriteProperty("w",v.w);
				w.WriteObjectEnd();
			});

      // -- Matrix4x4
      JsonMapper.RegisterExporter<Matrix4x4>((v,w) => {
        w.WriteObjectStart();
        w.WriteProperty("m00",v.m00);
        w.WriteProperty("m01",v.m01);
        w.WriteProperty("m02",v.m02);
        w.WriteProperty("m03",v.m03);
        w.WriteProperty("m10",v.m10);
        w.WriteProperty("m11",v.m11);
        w.WriteProperty("m12",v.m12);
        w.WriteProperty("m13",v.m13);
        w.WriteProperty("m20",v.m20);
        w.WriteProperty("m21",v.m21);
        w.WriteProperty("m22",v.m22);
        w.WriteProperty("m23",v.m23);
        w.WriteProperty("m30",v.m30);
        w.WriteProperty("m31",v.m31);
        w.WriteProperty("m32",v.m32);
        w.WriteProperty("m33",v.m33);
        w.WriteObjectEnd();
      });

			// -- Quaternion
			JsonMapper.RegisterExporter<Quaternion>((v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("x",v.x);
				w.WriteProperty("y",v.y);
				w.WriteProperty("z",v.z);
				w.WriteProperty("w",v.w);
				w.WriteObjectEnd();
			});

			// -- Color
			JsonMapper.RegisterExporter<Color>((v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("r",v.r);
				w.WriteProperty("g",v.g);
				w.WriteProperty("b",v.b);
				w.WriteProperty("a",v.a);
				w.WriteObjectEnd();
			});

			// -- Color32
			JsonMapper.RegisterExporter<Color32>((v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("r",v.r);
				w.WriteProperty("g",v.g);
				w.WriteProperty("b",v.b);
				w.WriteProperty("a",v.a);
				w.WriteObjectEnd();
			});

      // -- Bounds
      JsonMapper.RegisterExporter<UnityEngine.Bounds>((v,w) => {
				w.WriteObjectStart();

				w.WritePropertyName("center");
				writeVector3( v.center, w );

				w.WritePropertyName("size");
				writeVector3( v.size, w );

				w.WriteObjectEnd();
			});

			// -- Rect
			JsonMapper.RegisterExporter<Rect>((v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("x",v.x);
				w.WriteProperty("y",v.y);
				w.WriteProperty("width",v.width);
				w.WriteProperty("height",v.height);
				w.WriteObjectEnd();
			});

			// -- RectOffset
			JsonMapper.RegisterExporter<RectOffset>((v,w) => {
				w.WriteObjectStart();
				w.WriteProperty("top",v.top);
				w.WriteProperty("left",v.left);
				w.WriteProperty("bottom",v.bottom);
				w.WriteProperty("right",v.right);
				w.WriteObjectEnd();
			});

		}

	}
}
#endif
