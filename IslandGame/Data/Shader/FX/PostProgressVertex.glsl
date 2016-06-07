// shadertype=glsl
#version 400

const vec2[] vertices = vec2[]( vec2(-1, 1),
                                vec2(-1,-1),
                                vec2( 1, 1),
                                vec2( 1, 1),
                                vec2(-1,-1),
                                vec2( 1,-1));

out vec2 texCoords;
flat out int id;

void main() {
    gl_Position = vec4(vertices[gl_VertexID],0,1);
    texCoords = (vertices[gl_VertexID] + vec2(1,1))/2;
	id = gl_InstanceID;
}
