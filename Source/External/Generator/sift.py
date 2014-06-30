import string
import types
import os, glob
import shutil
from collections import deque
from os.path import basename

def read_file(filename):
	file = open(filename,"r")
	lines = []
	for line in file:
		line = line.rstrip('\n')
		lines.append(line)
	file.close()
	return lines
			   
def write_class(data, gen_file, gen_obj, gen_additional):
	file = data["file"]
	lines = gen_file(data, gen_obj, gen_additional)
	write = open(file,"w")
	for line in lines:
		print line
		write.write(line)
		write.write("\n")
	write.close()
	
def get_header(cat, data):
	name = data["headername"]
	data["properties"] = prepare_properties(data["fields"])
	
	cat += "\n~" + name + "\n"
	for prop in data["properties"]:
		cat += prop["ids"] + ","
	cat += "\n"
	return cat
	  
def gen_props(list):
	cat = "\n"
	count = 0
	for p in list:
		if p["comment"] != "":
			cat += "/// <summary> \n"
			cat += "/// " + p["comment"] + "\n"
			cat += "/// </summary> \n"
		count += 1
		cat += "[XmlElement(\"p"+str(count)+"\")] \n public %(type)s %(name)s { get; set; }\n" % p
	return cat
	
def gen_constructor_params(list):
	cat = ""
	for p in list:
		p["camel_case_name"] = camel_case(p["name"])
		cat += "%(type)s %(camel_case_name)s, " % p
	cat = cat[0:-2]
	return cat
	
def gen_constructor_actual_params(list):
	cat = ""
	for p in list:
		p["camel_case_name"] = camel_case(p["name"])
		cat += "%(camel_case_name)s, " % p
	cat = cat[0:-2]
	return cat
	
def gen_constructor_assigns(list):
	cat = "\n"
	for p in list:
		cat += "%(name)s = %(camel_case_name)s;  \n" % p
	return cat
	
def gen_usings(list):
	cat = ""
	for p in list:
		cat += "\nusing %(lib)s;" % {"lib" : p[0]}
	return cat
	
def pascal_case(s, sep='_'):
	global casing
	if(casing == 0):
		return s
	nl = []
	list = [t.title() for t in s.split(sep)]
	for l in list:
		if l == "Id":
			l = "ID"
		nl += [l]
	return ''.join(nl)
	
def camel_case(str):
	return str[0:1].lower() + str[1:]
	
def prepare_properties(list):
	newlist = []
	hascomment = 0
	comment = ""
	for	p in list:
		if len(p) == 0:
			continue
		if p[0] == "//":
			hascomment = 1
			p = p[1:len(p)]
			comment = ' '.join(p)
			continue
		
		
		newlist.append(
		{
			"comment":(comment if hascomment else ""), 
			"name":pascal_case(p[1]), 
			"ids":(p[1] if len(p) == 2 else p[2]), 
			"type":p[0]
		})
		if hascomment:
			hascomment = 0
	
	return newlist

def generate_orders(name, classdata):
	lines = []
	data = {}
	
	
	data["properties"] = prepare_properties(classdata)
	data["props"] = gen_props(data["properties"])
	data["ctor_params"] = gen_constructor_params(data["properties"])
	data["ctor_actual_params"] = gen_constructor_actual_params(data["properties"])
	data["ctor_assigns"] = gen_constructor_assigns(data["properties"])
	data["name"] = pascal_case(name)
	
	lines.append(
	"""
	public partial class %(name)s : Order
	{
		%(props)s
		
		public %(name)s () : base()
		{
		}
		
		public %(name)s ( int id, int factionID, int turn, %(ctor_params)s ) : base(id, factionID, turn)
		{
			%(ctor_assigns)s
		}
	}

    public static partial class OrdersExtensionMethods
    {
        public static void Add%(name)s(this FactionOrders me, int id, %(ctor_params)s )
        {
            me.Order(me.Owner.%(name)ss, id, new HexOrder(id, me.FactionID, G.Game.GameTurn, %(ctor_actual_params)s ));
        }
        public static bool Has%(name)s(this FactionOrders me, int id)
        {
			return me.Owner.%(name)ss.ContainsKey(id);
        }
        public static %(name)s %(name)s(this FactionOrders me, int id)
        {
			return me.Owner.%(name)ss[id];
        }
    }
	""" % data )
	
	return lines
	
def generate_structure(name, classdata):
	lines = []
	data = {}
	
	fullname = name.split("-")
	if(len(fullname) == 1):
		name = fullname[0]
		data["type"] = "struct"
	else:
		name = fullname[1]
		data["type"] = fullname[0]
		
	data["properties"] = prepare_properties(classdata)
	data["props"] = gen_props(data["properties"])
	data["ctor_params"] = gen_constructor_params(data["properties"])
	data["ctor_assigns"] = gen_constructor_assigns(data["properties"])
	data["name"] = pascal_case(name)
	data["constraint"] = " : this()" if data["type"] == "struct" else ""
	data["empty_ctor"] = (" public %(name)s () {} " % data) if data["type"] == "class" else ""
	
	
	lines.append(
	"""
	public %(type)s %(name)s
	{
		%(props)s
		
		%(empty_ctor)s
		
		public %(name)s ( %(ctor_params)s ) %(constraint)s
		{
			%(ctor_assigns)s
		}
	}
	""" % data )
	
	return lines

	
def generate_file(data, func, another_func):
	lines = []
	data["usings"] = gen_usings(data["using"]) if "using" in data else ""
	
	global casing
	casing = 1
	if "casing" in data and data["casing"] == "no":
		casing = 0
		
	lines.append(
	"""
	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

%(usings)s

namespace %(namespace)s
{
	
	""" % data )
		
	for struct in data:
		if not struct in ["namespace", "usings", "using", "file", "casing"]:
			lines = lines + func(struct, data[struct])
	lines += ["}"]
	
	return lines

	
	
def empty():
	return []
	
def process_file(file, base):
	lines = read_file("classes\\" + file)
	dict = {}
	list = []
	for line in lines:
		if line == "":
			continue
		elif " = " in line:
			s = line.split(" = ")
			dict[s[0]] = s[1]
		elif not (" " in line):
			dict[line] = []
			list = dict[line]
		else:
			sublist = filter(None, line.split(" "))
			list.append(sublist)
	if not "file" in dict:
		dict["file"] = base + ".cs"
	return dict
		
		
		
for subdir, dirs, files in os.walk(os.getcwd() + "\\classes"):
	for file in files:
		if file.endswith(".txt"):
			base = basename(file).split(".")[0]
			dict = process_file(file, base)
			if base == "Structures":
				write_class(dict, generate_file, generate_structure, empty)
			if base == "GcmSharedStructures":
				write_class(dict, generate_file, generate_structure, empty)
			if base == "GcmJsonParameters":
				write_class(dict, generate_file, generate_structure, empty)
			elif base == "Orders":
				write_class(dict, generate_file, generate_orders, empty)

		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
			