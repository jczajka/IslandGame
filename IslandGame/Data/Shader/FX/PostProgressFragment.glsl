// shadertype=glsl
#version 420

#define FXAA

out vec4 out_color;

layout(binding = 0) uniform sampler2D image;
layout(binding = 1) uniform sampler2D bloom;
layout(binding = 2) uniform sampler2D velocity;

uniform bool fxaa = true;

in vec2 texCoords;

vec3 saturation(vec3 rgb, float adjustment){
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    vec3 intensity = vec3(dot(rgb, W));
    return mix(intensity, rgb, adjustment);
}

vec3 fromGamma(vec3 color);

void main() {
	
	#ifdef FXAA
	if(fxaa){
		float fxaa_max_span = 4.0;
		float fxaa_min_reduce = 1.0/128.0;
		float fxaa_mul_reduce = 1.0/8.0;
		
		vec2 screen = textureSize(image, 0);
		vec3 luma = vec3(0.229, 0.587, 0.114);
		float lumaTL = dot(luma, texture(image, (gl_FragCoord.xy + vec2(-1,-1))/screen).xyz);
		float lumaTR = dot(luma, texture(image, (gl_FragCoord.xy + vec2( 1,-1))/screen).xyz);
		float lumaBL = dot(luma, texture(image, (gl_FragCoord.xy + vec2(-1, 1))/screen).xyz);
		float lumaBR = dot(luma, texture(image, (gl_FragCoord.xy + vec2( 1, 1))/screen).xyz);
		float lumaM  = dot(luma, texture(image, (gl_FragCoord.xy + vec2( 0, 0))/screen).xyz);
		
		vec2 dir = vec2(-((lumaTL + lumaTR) - (lumaBL + lumaBR)), (lumaTL + lumaBL) - (lumaTR + lumaBR));
		dir *= 1.0/min(abs(dir.x),abs(dir.y) + max((lumaTL + lumaTR + lumaBL + lumaBR) / 4 * fxaa_mul_reduce, fxaa_min_reduce));
		dir = clamp(dir, vec2(-fxaa_max_span), vec2(fxaa_max_span));
		
		vec3 result1 = (1.0/2.0) * (texture(image, (gl_FragCoord.xy + dir * vec2(1.0/3.0 - 0.5))/screen).xyz + texture(image, (gl_FragCoord.xy + dir * vec2(2.0/3.0 - 0.5))/screen).xyz);
		vec3 result2 = result1 * (1.0/2.0) + (1.0/4.0) * (texture(image, (gl_FragCoord.xy + dir * vec2(0.0/3.0 - 0.5))/screen).xyz + texture(image, (gl_FragCoord.xy + dir * vec2(3.0/3.0 - 0.5))/screen).xyz);
		
		float lumaMin = min(lumaM, min(min(lumaTL, lumaTR), min(lumaBL, lumaBR)));
		float lumaMax = max(lumaM, max(max(lumaTL, lumaTR), max(lumaBL, lumaBR)));
		float lumaResult2 = dot(luma, result2);
		if(lumaResult2 < lumaMin || lumaResult2 > lumaMax){
			result2 = result1;
		}
		out_color = vec4(result2,1);
	}else{
		out_color = texture(image, texCoords);
	}
	#else
		out_color = texture(image, texCoords);
	#endif
	
	out_color += texture(bloom, texCoords);


	
	vec2 v = texture(velocity, texCoords).xy;
	if(length(v) != 0){
		for(int i = 1; i < 8; i++)  {  
			out_color += texture(image, texCoords + v * i * 0.17);
		} 
		out_color /= 8;
	}

	float exposure = 1;
    out_color.rgb = fromGamma(vec3(1.0) - exp(-out_color.rgb * exposure));
}