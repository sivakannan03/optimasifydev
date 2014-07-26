<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:template match="/"> 
		<div id="tableContainer">
			<table  align="left" bgcolor="white" cellspacing="1" cellpadding="0"  class = "tablebg" isHeader="1" id="tblBrands" consider="1" startRow="1" parameter="DSARHeader" border="1">
				<!-- New Code -->	
				<tr class="tableheader" >
							<th colspan="5" align="right" nowrap="nowrap" >Daily Report at a glance (Regular)</th>
						<td colspan="4" align="left" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='HDate']/@HeaderDate" />
						</td>
				</tr>
				
				<tr class="tableheader" >
						<th colspan="1" align="right" nowrap="nowrap" >User Name : </th>
						<td colspan="2" align="left" nowrap="nowrap">
              <b>
							<xsl:value-of select="//row[@Element='UName']/@UserName" />
              </b>
						</td>
						<td colspan="2" align="right" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='ActType']/@VisitType" />
						</td>
						<td colspan="1" align="left" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='FDate']/@FetchDate" />
						</td>
						<td colspan="2" align="right" nowrap="nowrap">Submitted On:</td>
						<td colspan="1" align="right" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='SDate']/@SubmittedDate" />
						</td>
				
				</tr>
				<!-- End -->
		
			<tr class="tableheader" align="center">
					<th >Joint Work(PSO Name)</th>
            <th >Focus Doctors</th>
              <th>MCL Doctors</th>
                <th  >Specialty</th>
                  <th  >Call</th>
                    <th  >PCL Doctors</th>
                      <th  >MCL Chemist</th>
                        <th  >Call</th>
                          <th >PCL Chemist</th>
			</tr>
				 
				<xsl:choose>
					<xsl:when test="count(//row[@Element='DRPSO']/@Mon) &gt;0" >		
						<xsl:apply-templates select="//row[@Element='DRPSO']"/>
					</xsl:when>
					<xsl:otherwise>
						<tr class="tablecontent1"> 
							<td align="center"  colspan="9" >No Records Found !</td>
						</tr>
					</xsl:otherwise>
				</xsl:choose>
			</table>
		</div>
	</xsl:template>

	<xsl:template match="//row[@Element='DRPSO']">
	
				
		
		<tr class="tablecontent2">
			
			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@PSOName" />
			</td>
			
			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@MDrFD" />
			</td>

			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@MDrName" />
			</td>

			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@Specialty" />
			</td>

			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@MCDr" />
			</td>

			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@PDrName" />
			</td>
			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@MChName" />
			</td>

			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@MCCh" />
			</td>

			<td align="left"  class="tb-r1" nowrap="nowrap">
				<xsl:value-of select="@PChName" />
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>