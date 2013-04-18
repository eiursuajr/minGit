
  Ektron.ready(
    function()
    {
        if ("undefined" == typeof Ektron.MicroMessage)
        {
            Ektron.MicroMessage = 
            {
                // Properties
                currentMessageId: 0,
                replyAlertresource: "",
                searchTab:"",
                currentSearchText:"",
                
                // Methods
                CenterModal: function(modalSelector)
                {
                    $ektron(modalSelector).css("margin-top", -1 * Math.round($ektron(modalSelector).outerHeight()/2));
                },
                CreateModal: function(id,commentResource,maxResource,cancelResource,replyResource)
                {
                    var modalId = "MicroMessageModal" + id;
                    var modalSelector = "#" + modalId;
                    var thisMicroMessage = $ektron(modalSelector);
                    
                    if (thisMicroMessage.length == 0)
                    {
                        var pageBody = $ektron("body");
                        var modalHtml = '<div class="ektronWindow ektronModalStandard ektronMicroMessageModal" id="' + modalId + '">';
                        modalHtml += '  <div class="ektronModalHeader">';
                        modalHtml += '      <h3>';
                        modalHtml += '          <span class="headerText">'+ replyResource +'</span>';
                        modalHtml += '          <a href="#Close" class="ektronModalClose" title="Close"><span style="visibility:hidden;">Close</span></a>';
                        modalHtml += '      </h3>';
                        modalHtml += '  </div>';
                        modalHtml += '  <div class="ektronModalBody">';
                        modalHtml += '      <div class="ektronModalHeight-10">';
                        modalHtml += '          <p class="messages"></p>';
                        modalHtml += '          <div class="ekMicroMessageReplyPanelComment"><h4>'+commentResource+'</h4><blockquote>Comment</blockquote></div>';                    
                        modalHtml += '      </div>';
                        modalHtml += '      <br />';
                        modalHtml += '      <h4>'+ replyResource +'</h4>';
                        modalHtml += '      <p class="ekMicroMessageReplyTextAreaWrapper">';
                        modalHtml += '         <textarea id="ReplyContributionText'+ id + '"  name="ReplyContributionText'+ id +'"  class="ReplyContributionText" rows="5" maxlength="2000"></textarea>';
                        modalHtml += '      </p>';
                        modalHtml += '      <span class="ekMicroMessageCaption">'+ maxResource +'</span>';
                        modalHtml += '      <div class="ektronModalButtonWrapper clearfix">';
                        modalHtml += '          <ul class="buttonWrapper clearfix">';
                        modalHtml += '              <li><a class="button buttonRight redHover cancelButton" onclick="$ektron(\'' + modalSelector + '\').modalHide();return false;">'+ cancelResource +'</a>';
                        modalHtml += '                  <a class="button buttonRight greenHover ekMicroMessageReplyButton" onclick="Ektron.MicroMessage.AddReply(\'' + id + '\' );return false;">'+ replyResource +'</a></li>';
                        modalHtml += '          </ul>';
                        modalHtml += '      </div>';
                        modalHtml += '  </div>';
                        modalHtml += '</div>';
                        
                        // prepend the modal HTML to the body
                        pageBody.prepend(modalHtml);
                        theModal = $ektron(modalSelector);

                        // make the modal
                        theModal.modal({
                            modal: true,
                            toTop: true,
                            onShow: function(hash) {
                                $ektron(modalSelector).find("textarea").val("");
                                $ektron(modalSelector).find(".messages").empty();
                                Ektron.MicroMessage.CenterModal(modalSelector);
			                    hash.o.fadeTo("fast", 0.5, function() {
				                    hash.w.fadeIn("fast");
			                    });
                            },
                            onHide: function(hash) {
                                hash.w.fadeOut("fast");
			                    hash.o.fadeOut("fast", function() {
				                    if (hash.o)
				                    {
					                    hash.o.remove();
			                        }
			                    });
                            }
                        });
                      
                    }
                },
                Init: function(id, commentResource, maxCharacterResource, cancelResource, replyResource)
                {
                    Ektron.MicroMessage.CreateModal(id,commentResource, maxCharacterResource, cancelResource,replyResource);                    
                },
                SetReply: function(modalSelector, messageId, comment,searchTab,searchText)
                {
                    
                    var modal = $ektron("#MicroMessageModal"+ modalSelector);
                    var blockquote = modal.find("blockquote");
                    blockquote.html("<span class='bqStart'>&#8220;</span>" + comment + "<span class='bqEnd'>&#8221;</span>");
                    Ektron.MicroMessage.currentMessageId = messageId;
                    Ektron.MicroMessage.searchTab = searchTab;
                    Ektron.MicroMessage.currentSearchText=searchText;
                    modal.modalShow();
                    return false;
                },
                ShowSearch: function(id,mode)
                {
                    var searchDivId = "EkMicroMessageSearch" + id
                    var searchDiv = $ektron("#" + searchDivId);
                    var inputDiv = $ektron("#EkMicroMessageInput" + id);
                    var listDiv = $ektron("#EkMicroMessageDisplay"+ id);
                    
                    if(searchDiv != null && mode == "Search")
                    {
                        searchDiv.attr("style","display:block");
                        inputDiv.attr("style","display:none");
                        listDiv.attr("style","display:none");
                        $ektron("#dvGeneralTab"+id).removeClass('active');
                        $ektron("#dvSearchTab"+id).addClass('active');
                        $ektron("#ekMicroMessagingPaging"+id).attr("style","visibility:hidden");
                        $ektron("#ContributionText"+id).val('');
                        
                    } 
                    else
                    {
                        searchDiv.attr("style","display:none");
                        inputDiv.attr("style","display:block");
                        listDiv.attr("style","display:block");
                        $ektron("#dvSearchTab"+id).removeClass('active');
                        $ektron("#dvGeneralTab"+id).addClass('active');
                        $ektron("#ekMicroMessagingPaging"+id).attr("style","visibility:visible");
                         
                    }
                    return false;
                },
                AddReply: function(uniqueId)
                {
                    
                    var replyText = '';    
                    var modal = $ektron("#MicroMessageModal"+ uniqueId);
                    var replyTextArea = $ektron('#ReplyContributionText'+ uniqueId ); 
                                       
                    if (replyTextArea.length > 0)
                     {
                       replyText = replyTextArea.val();
                     }
                    
                    if (replyText == '')
                    {
                      
                      var message = Ektron.MicroMessage.replyAlertresource;
                      messageContainer= modal.find(".messages");
                      messageContainer.empty();
                      messageContainer.html('<span class="error">' + message + "</span>").fadeIn("slow");
                       return;
                    }
                    replyText=escape(replyText);
                    var functionName ='_PostReplyMicroMessage' + uniqueId;
                    (window[functionName])(IAjax.getArguements(),'control=' + uniqueId + '&ReplyContributionText' + uniqueId + '=' + replyText + '&messageId='+ Ektron.MicroMessage.currentMessageId + '&searchTab' + uniqueId + '=' + Ektron.MicroMessage.searchTab + '&__ecmmsgaction' + uniqueId + '=replymessage');
                    modal.modalHide();
                }
            }
            
        }
    }
);


