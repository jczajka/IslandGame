// shadertype=glsl
#version 420

out vec4 out_color;
in vec2 texCoords;

layout(binding = 0) uniform sampler2D image;

uniform int horizontal;

uniform float weight[11] = float[] (0.082607,	0.080977,	0.076276,	0.069041,	0.060049,	0.050187,	0.040306,	0.031105,	0.023066,	0.016436,	0.011254);

void main(){             
    vec2 tex_offset = 1.0 / textureSize(image, 0); // gets size of single texel
    vec3 result = texture(image, texCoords).rgb * weight[0]; // current fragment's contribution
    for(int i = 1; i < 11; ++i){
		result += texture(image, texCoords + vec2(tex_offset.x * i, tex_offset.y * i) * vec2(1-horizontal,horizontal)).rgb * weight[i]; 
        result += texture(image, texCoords - vec2(tex_offset.x * i, tex_offset.y * i) * vec2(1-horizontal,horizontal)).rgb * weight[i]; 
    }
    out_color = vec4(result, 1.0);
}