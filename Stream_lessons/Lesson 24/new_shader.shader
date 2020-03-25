Shader "Custom/new_shader" // название шейдера . меняем здесь
{
    Properties // блок свойств 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {} // 2D - текстура  
    }
    SubShader  // может быть несколько. какой выполниться - зависит от платформы. если не выполниться - перейдёт к следующему
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM // начало программы 
        // Physically based Standard lighting model, and enable shadows on all light types
        // заменяем стандартную на нашу кастомную Standard -> SimpleSpecular
        //  #pragma surface surf Standard fullforwardshadows  
        #pragma surface surf SimpleSpecular

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        // дублируем переменные чтобы шейдер мог считать их 
        sampler2D _MainTex;
        fixed4 _Color;
        struct Input // передаётся дальше в функцию. И с помощью этого получаем текущие координаты текстуры на модели
        {
            float2 uv_MainTex;
        };

        //half4 Lighting<Name> (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten);
        // https://docs.unity3d.com/ru/current/Manual/SL-SurfaceShaderLighting.html
        // логика модели освещения - она как-то влияет на текстуру нашего объекта
        half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten){
            // SurfaceOutput s - данные о поверхности 
            // viewDir - направление взгляда
            
           half d = dot(s.Normal , viewDir); // dot возвращает от 0 до 1. Если мы видим область - то вернёт 1. если не видим - 0.
           if (d < 0.3) d = 0;

           half sh = dot(s.Normal , lightDir); // берем тень
           sh = sh * 0.5 + 0.5; // немного высветляем
           if (sh < 0.02) sh = 0; // делаем тень более резкую

           // создаём переменную
           half4 col; 
           // присваиваем ей значение нашей текстуры
           col.rgb = s.Albedo * d * sh;  
           col.a = s.Alpha;
           return col; 
        }
        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o) // меняем структуру на нашу  SurfaceOutputStandard -> SurfaceOutput
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse" // если ничего сверху не выполнилось
}