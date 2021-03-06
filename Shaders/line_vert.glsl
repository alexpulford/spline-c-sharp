#version 430
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

out vec3 FragPos;
out vec3 Color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec4 fPos = vec4(aPos, 1.0) * model;
    FragPos = vec3(fPos);
    Color = aColor;  
    
    gl_Position = fPos * view * projection;
}