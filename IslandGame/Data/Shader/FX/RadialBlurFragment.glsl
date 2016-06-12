// shadertype=glsl
#version 420

out vec4 color;

layout(binding = 4) uniform sampler2D image;

in vec2 texCoords;

uniform vec2 lightpos;

void main() {


	vec2 lc = vec2(clamp(lightpos.x, 0, 1), clamp(lightpos.y, 0, 1));

	float SumColor = 0;
	vec2 TexCoord = texCoords;
	TexCoord += - lc.xy;
 
	const int sampels = 60;

	for (int i = 0; i < sampels; i++) {
		float scale = 1.0 - (float(i) / (sampels-1));
		SumColor += texture(image, TexCoord * scale + lc.xy).r;
	}
 
	color = vec4(vec3(SumColor / sampels),1);
}