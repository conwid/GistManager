# Immediate
Change save system. Autosave is quite tricky because can still select other gists whilst saving. Have a check system instead:
1.Change filename and comments events (loose enter, just keep lost focus) - this will update "GistEdited" bool in the manager (mirrors the current code editor system). 

Present user with prompt asking if they're sure they want to change or something. Basically encourage them back to manual save button. 

May have to intercept at the left click level in the selector rather than off the GistChanged event
