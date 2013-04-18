if(typeof Ektron == "undefined")
    Ektron = {};

if(typeof Ektron.PageTree == "undefined")
    Ektron.PageTree = 
    {
        Init : function (id, handlerPath, callback)
        {
            // gaurd against RegisterJSBlock problem; registers line twice with IE:
            if ("undefined" == typeof window["Ektron_PageTree_Initialized" + id]){
                window["Ektron_PageTree_Initialized" + id] = true;
            } else {
                return;
            }
            
            var root = $ektron("#"+id);
            var folderId = 0;
            
            $ektron.ajax({
                cache: false,
                url: handlerPath + "?fid=" + folderId,
                success: function(data,type)
                {
                    var branches = $ektron(data);
                    Ektron.PageTree.InitBranches(branches, callback);
                    
                    root.append(branches);
                    root.treeview({
                        collapsed : true,
                        toggle : function(evt)
                        {
                            var q = $ektron(this);
                            
                            if(q.attr("processed") != "true")
                            {
                                $ektron.ajax({
                                    cache: false,
                                    url: handlerPath + "?fid=" + q.children(".folder").attr("folder-id"),
                                    success: function(data,type)
                                    {
                                        var branches = $ektron(data);
                                        Ektron.PageTree.InitBranches(branches, callback);
                                        q.attr("processed", "true");
                                        q.find("> ul").append(branches);
                                        root.treeview({add:branches});
                                    },
                                    error: function(xhr,b)
                                    {
                                        alert(xhr.responseText);
                                    }
                                });
                            }
                        }
                    });
                },
                error: function(a,b)
                {
                }
            });
        },
        
        InitBranches : function(branches, callback)
        {
            branches.find("span.file").click(function(evt)
            {
                callback($ektron(this).attr("page-id"), $ektron(this).html());
            });
        }
    };