Getting Started with Manos
==========================

<<<<<<< Updated upstream
This is page two of the Getting Started with Manos tutorial.  It assumes you already have
Manos installed and have created the Hello World App from page one.
=======

Setting up the Routes
---------------------

For our application we are going to want four main routes:
>>>>>>> Stashed changes


Templates Overview
------------------

Normally we want to spit out something other than just plain strings from our actions. Usually in a web
application we want to create HTML or maybe JSON.  Instead of forcing you to create these formats from hand,
Manos includes a powerful templating engine that can convert a .NET object into a document.

![Diagram showing an object being converted to a document](http://github.com/jacksonh/manos/raw/master/docs/tutorial/manos-template-engine-flow.png)

<<<<<<< Updated upstream
=======
    [Route ("/r/{id}~")]
    public void LinkInfo (Shorty app, IManosContext ctx, string id)
    {
    }
>>>>>>> Stashed changes

Creating the Hello World Template
---------------------------------

To create our Hello World template we need to create an index.html page in our Templates/ directory and add the following text:

    <html>
      <head>
        <title>Hello World!</title>
      </head>
      <body>
         Hello World!
      </body>
    </html>


Rendering our Hello World Template
----------------------------------

Because Manos's template engine turns templates into CLR objects, we can now call our templates render method from our action.  Lets
update the action code to look like this:

    Get ("/", ctx => ctx.Render ("index.html"));


Sending Data to our Templates
-----------------------------

In the last two examples you might have wondered what the null parameter is for.  This parameter is how we pass data into the
template engine.  When templates are rendered, the template engine will try to resolve properties on the supplied data.  So if we want
to pass a name into our template, all we need to do is this:

    Get ("/", ctx => ctx.Render ("index.html", new { Name = "Manos" }));

and update our template to use that name:

    <html>
      <head>
        <title>Hello World!</title>
      </head>
      <body>
         Hello {{ Name }}!
      </body>
    </html>

The {{ }} brackets tell the template engine to render the supplied expression to a string. In this case our expression is a variable
named Name, that happens to be a string property on the supplied object.  Variables can also be defined in the template or be part of
an expression, like a for loop:

defined variable:
    ....
      {% set LastName = "World" %}
      <body>
        Hello {{ Name }} {{ LastName }}!
      </body
    ....

for loop:
    ....
      <body>
        <ul>
          {% for item in collection %} 
            <li>{{ item }}</li>  
          {% endfor %}
        </ul>
      </body>
    ....


Building and Running Templates
------------------------------

To build and run our templates, we'll use the manos command again.

    manos -build
    manos -run

if we navigate to http://localhost:8080/ we get a real web page.  Viewing the source should look something like this:

    <html>
      <head>
        <title>Hello World!</title>
      </head>
      <body>
         Hello Manos!
      </body>
    </html>

Template Inheritance
--------------------

Most sites will look almost exactly the same on every single page. Rather than update every page every time we change
the site's layout, it would be nice to share that structure between all of our pages.  Manos makes this easy with
template inheritance. Template inheritance allows you to create a basic page that will be shared between a number of
pages and add blocks to your base page that can be set by each individual page.  Here is our Hello World example rewritten
to use a base page.

Templates/base.html:

    <html>
      <head>
        <title>{% block title %}Hello{% endblock %}</title>
      </head>
      <body>
        {% block content %}
          This page has no content.
        {% endblock %}
      </body>
    </html>

Templates/index.html:

    {% extends "base.html" %}

    Since we are using an extends statement,
    everything outside of code statements will
    be discarded.

    {% block title %}Hello World!{% endblock %}

    {% block content %}
      Hello {{ Name }}!
    {% endblock %}



Template Operations
------------------

Here are some of the cool things you can do with Manos's template engine. For a more in depth
look at the templating engine, checkout the Templates Guide.

### Filters
Filters allow you to easily convert text from one format to another. For example, using the uppercase filter:

    {{ "hELlo" | uppercase }}

will create this:

    HELLO

There are a number of useful filters for rendering markdown, extracting parts of a date or time and parsing URLs. All
of the available filters are listed in the Templates Guide. If the supplied filters don't meet your needs, you can
always add your own filters.

### Conditional statements
Manos supports if, elif and else statements.

    {% if show_name %}
      {{ Name }}
    {% else %}
      Sorry we can't show you the name.
    {% endif %}


### Macros
Macros are a lot like methods in C#.  They allow you to encapsulate and reuse code easily.  Macros can accept parameters
and support default values for parameters.

    {% macro print_button (name, style='big') %}
      <input type="button" name="{{ name }}" style="{{ style }}"></input>
    {% endmacro %}

    ....

    {% print_button ('small-button', 'small') %}
    {% print_button ('big-button') %}

### Includes
Includes allow you to include another file into your template.  Includes can be evaluated at build time or at runtime,
depending on the supplied value.

This will be evaluated at build time:

    {% include "some-file.html" %}
    
And this will be evaluated at run time:

    {% include some_variable %}

### Imports
If you have a bunch of macros that you would like to share between templates you can put them in a common file and
use the import statement:

    {% include "macros.html" as macros %}

    ....
    {% macros.print_calendar () %}


Moving On
---------

Now that we have a good idea of how templates work, we'll go over adding unit tests to your code in part three.

Note that we used a couple different ways of setting up routes here. Our Index method doesn't
need any parameters, so it simply implements the ManosAction delegate. It also takes advantage
of Route's params constructor to route a few different strings to itself.

The second method SubmitLink will be called when we submit our form.  We don't really want people
accidently going to thise "page", so it will only accept POST requests. Note that we are recieving
the link as a parameter.  The link will be set in form data.

The last two methods use the simple routing syntax to map pieces of the request url to parameters.


Writing some HTML
-----------------

The first thing we need to do is create our home page. Since Manos's template engine is disabled
in this release, we need to write the HTML ourself (or we could use another template engine).

    [Route ("/", "/Home", "/Index")]
    public void Index (IManosContext ctx)
    {
        ctx.Response.WriteLine (@"<html>
                                   <head><title>Welcome to Shorty</title></head>
                                   <body>
                                    <form method='POST' action='submit-link'>
                                     <input type='text' name='link'>
                                     <input type='submit'>
                                    </form>
                                   </body>");
    }

It's not beautiful, but that will at least let us get some links into our application.


Storing our links
-----------------

For now, lets just assume our app will never crash and these links can live in memory
forever. That will let us use Manos's object cache for storing our links.

We'll also need to create a simple LinkData class to store our links in the cache, this
can either be a nested class or you can create a LinkData.cs file and stick it in there.

    public class LinkData {

	public string Link;
	public int Clicks;

	public LinkData (string link)
	{
		Link = link;
	}
    }

Finally, we need to create a hashing function for generating unique ids based on our URLs.

    private static string GenerateHash (string str, int length)
    {
	byte [] data = Encoding.Default.GetBytes (str);

	SHA1 sha = new SHA1CryptoServiceProvider (); 
	data = sha.ComputeHash (data);

	string base64 = Convert.ToBase64String (data);

        int i = 0;
	StringBuilder result = new StringBuilder ();
	while (result.Length < length) {
		if (Char.IsLetterOrDigit (base64 [i]))
			result.Append (base64 [i]);
		++i;
		if (i >= base64.Length)
			return null;
	}
	return result.ToString ();
    }


Now that we have that stuff out of the way, all we need to do is stick our id and LinkData in the
cache and then redirect the user to their LinkInfo page.

    [Post ("/submit-link")]
    public void SubmitLink (Shorty app, IManosContext ctx, string link)
    {
        string id = GenerateHash (link, 5);

	Cache [id] = new LinkData (link);
	ctx.Response.Redirect ("/r/" + id + "~");
    }


Displaying the Link Data
------------------------

Our LinkInfo method is pretty straight forward.  It looks up the suppiled id and displays its
corresponding data.  If no data is found, the user is given a 404 error.

    [Route ("/r/{id}~")]
    public void LinkInfo (Shorty app, IManosContext ctx, string id)
    {
        LinkData info = Cache [id] as LinkData;

        if (info == null) {
	    ctx.Response.StatusCode = 404;
	    return;
	}

	ctx.Response.WriteLine (@"<html>
                                   <head><title>Welcome to Shorty</title></head>
                                   <body>
                                    {0} was clicked {1} times.
                                   </body>", info.Link, info.Clicks);
    }


Handling the Redirection
------------------------

The only complicated thing in our redirection method is the way that we increment the
Clicks field. Manos runs HTTP transactions in parallel, so there is a chance
that another user is redirecting at the exact same time as us. To make sure our
Clicks field is incremented properly, we can use the
System.Threading.Interlocked.Increment method.

    [Route ("/r/{id}")]
    public void Redirector (Shorty app, IManosContext ctx, string id)
    {
	LinkData info = Cache [id] as LinkData;

	if (info == null) {
		ctx.Response.StatusCode = 404;
		return;
	}

	//
	// Because multiple http transactions could be occuring at the
	// same time, we need to make sure this shared data is incremented
	// properly
	//
	Interlocked.Increment (ref info.Clicks);

        ctx.Response.Redirect (info.Link);
    }


That's it!
----------
Manos still has a long ways to go, but hopefully this tutorial shows off some
of its potential. Future versions of Manos will be shaped by your comments and
suggestions, so please don't be afraid to offer advice.

The complete source code for this tutorial is available in the
examples/Shorty directory.
