﻿namespace MessageService.Abstractions;

public class EntityFilter
{
    public EntityFilter()
    {
    }

    public EntityFilter(string text)
    {
        Text = text;
    }

    public string Text { get; set; }
}