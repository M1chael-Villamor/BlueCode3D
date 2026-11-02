using System;
using System.Collections.Generic;

[Serializable]
public class Pregunta
{
    public int id;
    public string dominio;
    public string enunciado;
    public List<string> opciones;
    public int respuestaCorrecta;
}

[Serializable]
public class BancoPreguntas
{
    public List<Pregunta> preguntas;
}